using System;
using System.Threading;

namespace Daybot
{
  class Program
  {
    public static NumberHandler NumberHandlerInstance = new NumberHandler();

    static void Main(string[] args)
    {
      if (!SettingsLoader.Load()) {
        Console.WriteLine("press any key to exit...");
        Console.Read();
        return;
      }

      BackgroundWorker backgroundWorker = new BackgroundWorker();
      backgroundWorker.Start();

      Daybot.IRC.TwitchClient.Reconnect();

      try {
        while (true) {
          Thread.Sleep(100);
        }
      }
      catch(Exception e) {
        Console.WriteLine(e.Message);
        Console.WriteLine(e.StackTrace);
      }


      backgroundWorker.DoStop();
    }
  }
}
