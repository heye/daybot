# daybot
A Twitch chat bot that does statistics on recent messages.

How To Run:

1. Download daybot.zip from [latest release](https://github.com/heye/daybot/releases/tag/release) 
2. Unzip daybot.zip to any folder
3. Fill values in the example settings.json
  - Use [https://twitchtokengenerator.com/](https://twitchtokengenerator.com/) to generate a 'Bot Token'
  - Copy the 'Access Token' to the settings.json's 'TwitchToken'
5. Double click daybot.exe

If the bot connects successfully to twitch chat you should see output similar to this:
![Screenshot of the running bot](https://github.com/heye/daybot/blob/master/readme_screenshot.png?raw=true)



Typical Problems:
- Bot only shows 'TWITCH CLIENT - CONNECTING...' -> Restart the application
- Connection is succesfull but no messages are shown 
  - Does channel name include UPPER CASE characters? Try with only lower case. E.g. 'the_HEYE -> 'the_heye'
- You may need to install the required framework. Follow the link in the error message or try this one:
  - https://aka.ms/dotnet-core-applaunch?framework=Microsoft.NETCore.App&framework_version=3.1.0&arch=x64&rid=win-x64&os=win10
- To get error messages you can start a terminal in the downloaded directory and run 'daybot.exe'
