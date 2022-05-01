using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.Statistics;

namespace Daybot
{
  class NumberMessage
  {
    public int Number;
    public string Sender;
    public long Time;
  }

  class NumberOccurenceList
  {
    private List<NumberOccurenceEntry> NumberOccurenceEntries = new List<NumberOccurenceEntry>();

    public bool Contains(int Number)
    {
      for(int i=0; i< NumberOccurenceEntries.Count; i++) {
        if(NumberOccurenceEntries[i].Number == Number) {
          return true;
        }
      }
      return false;
    }

    public void AddOccurence(int Number)
    {
      if (!Contains(Number)) {
        NumberOccurenceEntry NewEntry = new NumberOccurenceEntry();
        NewEntry.Number = Number;
        NewEntry.Count = 1;
        NumberOccurenceEntries.Add(NewEntry);
      }
      else {
        for (int i = 0; i < NumberOccurenceEntries.Count; i++) {
          if (NumberOccurenceEntries[i].Number == Number) {
            NumberOccurenceEntries[i].Count += 1;
          }
        }
      }
    }

    public double GetMedian()
    {
      List<double> data = new List<double>();
      for (int i = 0; i < NumberOccurenceEntries.Count; i++) {

        //add each number COUNT times to the list for median computation
        for (int num = 0; num < NumberOccurenceEntries[i].Count; num++) {
          data.Add(NumberOccurenceEntries[i].Number);
        }
      }

      return data.Median();
    }

    public double GetMean()
    {
      List<double> data = new List<double>();
      for (int i = 0; i < NumberOccurenceEntries.Count; i++) {

        //add each number COUNT times to the list for median computation
        for (int num = 0; num < NumberOccurenceEntries[i].Count; num++) {
          data.Add(NumberOccurenceEntries[i].Number);
        }
      }

      return data.Mean();
    }

    public string GetMessage()
    {
      if (NumberOccurenceEntries.Count == 0) {
        return "";
      }

      string Message = "";
      Message += "Antworten(Anzahl): ";


      NumberOccurenceEntries.Sort(
        delegate (NumberOccurenceEntry e1, NumberOccurenceEntry e2) {
          return e1.Number.CompareTo(e2.Number);
        });

      for (int i = 0; i < NumberOccurenceEntries.Count && i < 5; i++) {
        Message += NumberOccurenceEntries[i].Number.ToString() + "(" + NumberOccurenceEntries[i].Count.ToString() + ") ";
        if(i + 1 < NumberOccurenceEntries.Count && i < 4) {
          Message += "-- ";
        }
      }

      Message += "|  ";
      Message += "Median(" + GetMedian().ToString("0.#") + ") Avg(" + GetMean().ToString("0.#") + ")";

      return Message;
    }
  }

  class NumberOccurenceEntry
  {
    public int Number = 1;
    public int Count = 1;
  }


  class NumberHandler
  {
    public int EvaluationCooldown = 20;
    public int MinNumberMessages = 5;
    private long LastEval = 0;
    private List<NumberMessage> RecentMessages = new List<NumberMessage>();

    private bool PostAgain = false;

    public int GetNumber(string message)
    {
      try {
        return int.Parse(message);
      }
      catch(Exception) {
      }
      try {
        return (int)float.Parse(message);
      }
      catch (Exception) {
      }

      return -1;
    }

    public string CheckCommand(string message, string sender)
    {
      if (message.StartsWith(":")) {
        message = message.Substring(1);
      }

      int num = GetNumber(message);
      if (num < 0) {
        //Console.WriteLine("MESSAGE IS NO NUMBER: " + num.ToString());
        //don't return anything on number ever (this is not a !xyz command)
        return "";
      }

      //Console.WriteLine("MESSAGE IS NUMBER: " + num.ToString());

      NumberMessage numberMessage = new NumberMessage();
      numberMessage.Number = num;
      numberMessage.Sender = sender;
      numberMessage.Time = Utils.Now();

      for (int i = 0; i<RecentMessages.Count; i++) {
        if(RecentMessages[i].Sender == sender) {
          //disable for testing to allow multiple entries per chatter
          //RecentMessages.Remove(RecentMessages[i]);
          break;
        }
      }
      PostAgain = true;
      RecentMessages.Add(numberMessage);

      //don't return anything on number ever (this is not a !xyz command)
      return "";
    }

    public void Tick()
    {
      Evaluate();
    }

    //TODO: run this in a thread? or trigger after message receival with a cooldown time?
    public void Evaluate()
    {
      //EvaluationCooldown seconds cooldown
      if (LastEval + SettingsLoader.Settings.NumberPostingCooldown > Utils.Now()) {
        return;
      }
      LastEval = Utils.Now();

      if(RecentMessages.Count < MinNumberMessages) {
        return;
      }

      if (!PostAgain) {
        return;
      }
      PostAgain = false;

      Console.WriteLine("EVALUATE " +RecentMessages.Count + " NUMBER MESSAGES");

      var NumberOccurence = new List<NumberOccurenceEntry>();
      // Key = Count of Occurences
      // Vaue = Number that Chatter wrote

      //if oldest message > 60s
      // -> reset completely
      long oldesMessageTime = Utils.Now();
      for (int i = 0; i < RecentMessages.Count; i++) {
        if (RecentMessages[i].Time < oldesMessageTime) {
          oldesMessageTime = RecentMessages[i].Time;
        }
      }
      if (oldesMessageTime + 60 < Utils.Now()) {
        Console.WriteLine("CLEAR NUMBER MESSAGES");
        RecentMessages.Clear();
        return;
      }

      //THERE ARE RECENT NUMBER MESSAGES
      //-> PROCESS FOR MEDIAN ETC.

      NumberOccurenceList numberOccurenceList = new NumberOccurenceList();
      for (int i = 0; i < RecentMessages.Count; i++) {
        numberOccurenceList.AddOccurence(RecentMessages[i].Number);
      }

      string message = numberOccurenceList.GetMessage();
      if(message.Length != 0) {
        IRC.TwitchClient.SendMessage(message);
      }
      Console.WriteLine(message);



      //TODO: do same for !preis command
      //TODO: console output in german?
    }
  }
}
