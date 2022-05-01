using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.IO;

namespace Daybot
{
  class SettingsSerializable
  {
    public string TwitchUser { get; set; } = "";
    public string TwitchToken { get; set; } = "";
    public string ChannelName { get; set; } = "";
    public bool EnablePosting { get; set; } = true;
    public int CooldownSeconds { get; set; } = 5;
  }

  class SettingsLoader
  {
    public static SettingsSerializable Settings = new SettingsSerializable();

    public static bool Load()
    {
      string fileName = "settings.json";
      string jsonString = File.ReadAllText(fileName);
      //Console.Write(jsonString);
      Settings = JsonSerializer.Deserialize<SettingsSerializable>(jsonString);

      return Validate();
    }

    public static bool Validate()
    {
      bool ret = true;
      if (Settings.TwitchToken.Length == 0) {
        Console.WriteLine("Settings Error: TwitchUser is empty");
        ret = false;
      }
      if (Settings.TwitchToken.Length == 0) {
        Console.WriteLine("Settings Error: TwitchToken is empty");
        ret = false;
      }
      if (Settings.ChannelName.Length == 0) {
        Console.WriteLine("Settings Error: ChannelName is empty");
        ret = false;
      }

      if (!ret) {
        Console.WriteLine("Check the example Settings file and modify it with your values.");
      }

      return ret;
    }
  }
}
