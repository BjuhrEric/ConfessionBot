# Common usage notes
* To confess, send a direct message to the bot starting with "confess" followed by your confession. E.g.
  * confess This community has the cutest members
* If the same bot is in multiple mutual servers a dialog will occur asking where to publish the confession.
* To obtain a link to the source code, send the following direct message to the bot (without quotation signs): "confessionsourcecode"

# Administrative usage notes
* This bot uses a prefix for configuring certain server specific settings. By default the configuration prefix is an exclamation mark, "!", but can be changed to any string desired.
* In order to use any of the administrative commands, the bot needs to have view channel and read message history permissions in the channel you post the commands.
* In order to have any confession posted, a confession publishing channel has to be set using the "setconfessionchannel" command.
* In order to log confessions for administrative use, a logging channel has to be set using the "setloggingchannel" command.
* Using either of the above commands without a parameter sets the corresponding channel to the current one.
* In order to post logs and confessions, the bot will require send messages permission in the assigned channel.
* The logs posted include the exact same Embed as the confession itself; with a message including a spoiler-hidden username and id, as well as a timestamp.

# Administrative commands
* setconfessionprefix (new prefix) - sets the prefixes for config commands
* confessionbotusageinfo (optional channel) - publishes usage information about the bot in either the current or given channel
* confessionbotconfiginfo (optional channel) - publishes config information about the bot in either the current or given channel
* setconfessionchannel (optional channel) - sets the channel in which the confessions will be published
* setconfessionlogchannel (optional channel) - sets the channel in which the confessions will be logged
* setventingchannel (optional channel) - sets the channel in which the venting will be published
* setventinglogchannel (optional channel) - sets the channel in which the venting will be logged
* clearconfessionchannel - clears the channel in which the confessions are published
* clearconfessionlogchannel - clears the channel in which the confessions are logged
* clearventingchannel - clears the channel in which the venting are published
* clearventinglogchannel - clears the channel in which the venting are logged

# Hosting notes
* Remember to keep the Config.xml and ConfigSchema.xsd files in the same directory as the executable file. 
* The config file contains the channels and prefixes associated with each server, as well as the Token for the bot.
* The schema file is necessary for validating the config file, and may only be changed when updating the bot.
* When updating to a new version of the executable, unless told otherwise, keep your own Config.xml file.
* In order to run the bot you will need to either:
  * Edit the token property in your Config.xml file to match that of your own bot (found in the Developer Portal).
  * Enter the token in the console terminal that opens with the bot upon running the executable.
* In order for the bot to operate correctly, make sure that the bot has Server Member intent enabled in the Developer Portal, and is invited to the server with the following permissions:
  * View Channels
  * Send Messages
  * Read Message History

# Building from source
* This project was made using Visual Studio 2019, and the developer will only offer support for this version at this time.
* The project requires Nuget components, C# build tools as well as .NET 5 or later.
* Future plans may include creating a CMake installation for the project, but as of yet this is not supported.
* The project makes heavy use of [SimpleDiscordBot](https://github.com/GoodGirlErica/SimpleBot) and [Discord.NET](https://github.com/discord-net/Discord.Net) which you may want to familiarize yourself with.
