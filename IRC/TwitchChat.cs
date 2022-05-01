using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daybot.IRC
{
  class TwitchClient
  {
    static IrcClient client = new IrcClient("irc.chat.twitch.tv", 6697, true);

    public static string CurrentChannel = "";
    public static string LastMessage = "";

    private static bool IsSetup = false;

    private static float LastConnect = 0;
    private static float ConnectRetryTime = 5;

    private static float LastWealthCommand = 0;
    private static float CommandTimeout = 5;

    private static long LastSend = 0;

    public static bool IsConnected()
    {
      //IRC CONNECTION MAY EXIST, BUT WITHOUT CHANNEL OR USER AUTHENTICATION WE WOULD HAVE DO RECONNECT 
      return CurrentChannel.Length > 0;
    }

    public static float Now()
    {
      return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
    }

    public static void CheckState()
    {
      if (!IsSetup) {
        //remove handlers because somehow onchannelmessage was sometimes triggered twice???
        client.OnConnect -= OnConnectHandler;
        client.OnConnect += OnConnectHandler;
        client.UserJoined -= UserJoinedHandler;
        client.UserJoined += UserJoinedHandler;
        client.ChannelMessage -= OnChannelMessage;
        client.ChannelMessage += OnChannelMessage;
        client.NoticeMessage -= OnNoticeMessage;
        client.NoticeMessage += OnNoticeMessage;
        client.PrivateMessage -= OnPrivateMessage;
        client.PrivateMessage += OnPrivateMessage;
        client.ServerMessage -= OnServerMessage;
        client.ServerMessage += OnServerMessage;
      }

      if (!IsConnected() && LastConnect + ConnectRetryTime < Now()) {
        Console.WriteLine("TWITCH CLIENT - CONNECTING...");
        LastConnect = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();


        //increase reconnect back-off only until maximum of 2 minutes
        if (ConnectRetryTime < 120) {
          ConnectRetryTime += 5;
        }

        client.ServerPass = "oauth:" + SettingsLoader.Settings.TwitchToken;
        client.Nick = "oauth:" + SettingsLoader.Settings.TwitchUser;

        //this is a new connection - reset old connection info
        CurrentChannel = "";
        LastMessage = "";

        client.Connect();
      }
    }

    public static void Reconnect()
    {
      client.Disconnect();
      CurrentChannel = "";
      LastMessage = "";
      LastConnect = 0; //reset reconnect back-off, because this may be a manual retry from the user
      CheckState();
    }

    public static void OnConnectHandler(object sender, EventArgs e)
    {
      Console.WriteLine("CONNECTED, JOINING CHANNEL " + SettingsLoader.Settings.ChannelName);
      string channel = Daybot.SettingsLoader.Settings.ChannelName;
      client.JoinChannel("#" + channel);
    }

    public static void UserJoinedHandler(object sender, UserJoinedEventArgs e)
    {
      Console.WriteLine("ON USER JOINED " + e.Channel);

      //reset reconnect back-off on completely successful connection
      ConnectRetryTime = 5;

      CurrentChannel = e.Channel;
    }

    public static void OnChannelMessage(object sender, ChannelMessageEventArgs e)
    {
      Console.WriteLine("ON CHANNEL MESSAGE " + e.From + ": " + e.Message);
      LastMessage = e.From + ": " + e.Message;

      CheckCommand(e.Message, e.From);
    }

    public static void CheckCommand(string message, string sender)
    {
      Program.NumberHandlerInstance.CheckCommand(message, sender);
    }

    public static void SendMessage(string message)
    {
      if(LastSend + SettingsLoader.Settings.GeneralPostingCooldown > Utils.Now()) {
        Console.WriteLine("BLOCKED SEND (cooldown): " + message + " " + LastSend.ToString() + " > " + Utils.Now().ToString());
        return;
      }
      LastSend = Utils.Now();


      if (!SettingsLoader.Settings.EnablePosting) {
        Console.WriteLine("BLOCKED SEND (not allowed): " + message + " " + LastSend.ToString() + " > " + Utils.Now().ToString());
        return;
      }

      Console.WriteLine("SEND: " + message);

      //disabled for safety while testing
      client.SendMessage("#" + SettingsLoader.Settings.ChannelName, message);
    }

    public static void OnNoticeMessage(object sender, NoticeMessageEventArgs e)
    {
      Console.WriteLine("ON NOTICE MESSAGE " + e.From + ": " + e.Message);
    }

    public static void OnPrivateMessage(object sender, PrivateMessageEventArgs e)
    {
      Console.WriteLine("ON PRIVATE MESSAGE " + e.From + ": " + e.Message);
    }

    public static void OnServerMessage(object sender, StringEventArgs e)
    {
      Console.WriteLine("ON SERVER MESSAGE " + e.Result);
    }
  }
}
