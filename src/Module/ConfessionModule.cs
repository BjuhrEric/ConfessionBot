#pragma warning disable RCS1090 // Add call to 'ConfigureAwait' (or vice versa).
namespace ConfessionBot.Module
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using ConfessionBot.res.strings;
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using SimpleDiscordBot.Module;

    public class ConfessionModule : SimpleBotBaseTopLevelModule
    {
        [Command("confess")]
        [Summary("Sends a confession for the bot to anonymously publish to the server.")]
        [Remarks("DM")]
        public async Task ConfessAsync([Remainder] string content)
        {
            if (Context.IsPrivate)
            {
                await this.TryToSendConfessionAsync(content, ConfessionType.LoveConfession);
            }
        }

        [Command("vent")]
        [Summary("Sends a vent message for the bot to anonymously publish to the server.")]
        [Remarks("DM")]
        public async Task VentAsync([Remainder] string content)
        {
            if (Context.IsPrivate)
            {
                await this.TryToSendConfessionAsync(content, ConfessionType.VentConfession);
            }
        }

        private static Embed CreateConfessionEmbed(string content, ConfessionType type)
        {
            var builder = new EmbedBuilder();
            var fieldBuilder = new EmbedFieldBuilder
            {
                Name = type.EmbedTitle,
                Value = content,
                IsInline = false
            };
            builder.AddField(fieldBuilder);
            return builder.Build();
        }

        private static bool IsValidChannelId(ulong id)
        {
            return id != ulong.MaxValue && id != 0;
        }

        private static int ParseChoice(SocketMessage msg)
        {
            int choice;
            try
            {
                choice = byte.Parse(msg.Content);
            }
            catch (Exception)
            {
                choice = int.MinValue;
            }

            return choice;
        }

        private static async Task PublishConfessionAsync(Embed embed, SocketGuild guild, ulong publishChannelId)
        {
            var publishChannel = guild.GetTextChannel(publishChannelId);
            await publishChannel.SendMessageAsync(embed: embed);
        }

        private static string ServerListText(SocketGuild[] guilds)
        {
            var sb = new StringBuilder(confessionbot_strings.ConfessionBotSelectServer);
            sb.AppendLine();
            for (int i = 0; i < guilds.Length; i++)
            {
                sb.Append(i).Append(". ").AppendLine(guilds[i].Name);
            }

            return sb.ToString();
        }

        private static SocketGuild[] ToGuildArray(IReadOnlyCollection<SocketGuild> guilds)
        {
            var guildArray = new SocketGuild[guilds.Count];
            var i = 0;
            foreach (SocketGuild guild in guilds)
            {
                guildArray[i] = guild;
                i++;
            }

            return guildArray;
        }

        private static bool WithinArrayBounds<T>(T[] guilds, int choice)
        {
            return choice >= 0 && choice < guilds.Length;
        }

        private async Task<int> AskHowToProceedAsync()
        {
            await this.ReplyAsync(confessionbot_strings.ConfessionBotMultipleMutualServers);
            return await this.GetHowToProceedAsync();
        }

        private async Task<int> AttemptToGetSpecificGuildAsync(SocketGuild[] guilds)
        {
            for (byte attempts = 0; attempts < 5; attempts++)
            {
                await this.SendServerListAsync(guilds);
                var choice = await this.GetChoiceFromUserAsync();
                if (ConfessionModule.WithinArrayBounds(guilds, choice))
                {
                    return choice;
                }

                await this.ReplyAsync(confessionbot_strings.ConfessionBotInvalidResponse);
            }

            return -1;
        }

        private async Task ConfessToGuildAsync(string content, ConfessionType type, SocketGuild guild)
             => await this.ConfessToGuildAsync(CreateConfessionEmbed(content, type), guild, type);

        private async Task ConfessToGuildAsync(Embed embed, SocketGuild guild, ConfessionType type)
        {
            var storage = this.GetServerStorage(guild.Id);
            var publishChannelId = storage.GetNamedID(type.PublishChannelName);
            var loggingChannelId = storage.GetNamedID(type.LoggingChannelName);
            if (ConfessionModule.IsValidChannelId(publishChannelId))
            {
                await ConfessionModule.PublishConfessionAsync(embed, guild, publishChannelId);

                if (ConfessionModule.IsValidChannelId(loggingChannelId))
                {
                    await this.LogConfessionAsync(embed, guild, loggingChannelId);
                }
            }
        }

        private async Task<int> GetChoiceFromUserAsync()
        {
            return ConfessionModule.ParseChoice(await this.NextMessageAsync());
        }

        private async Task<int> GetHowToProceedAsync()
        {
            for (byte attempts = 0; attempts < 5; attempts++)
            {
                await this.ReplyAsync(confessionbot_strings.ConfessionBotServerAmbiguity);
                var nextChoice = await this.GetChoiceFromUserAsync();
                if (nextChoice >= 1 && nextChoice <= 3)
                {
                    return nextChoice;
                }

                await this.ReplyAsync(confessionbot_strings.ConfessionBotInvalidResponse);
            }

            await this.ReplyAsync(confessionbot_strings.ConfessionBotAmbiguityFail);
            return -1;
        }

        private async Task HandleMutualGuildsAmbiguityAsync(string content, IReadOnlyCollection<SocketGuild> mutualGuilds, ConfessionType type)
        {
            // We need to know where to post this confession.
            var choice = await this.AskHowToProceedAsync();
            switch (choice)
            {
                case 1:
                    await this.PublishInSpecificGuildAsync(mutualGuilds, content, type);
                    break;
                case 2:
                    await this.PublishInAllMutualGuildsAsync(mutualGuilds, content, type);
                    break;
                case 3:
                    await this.ReplyAsync(confessionbot_strings.ConfessionBotConfessionAborted);
                    break;
                default:
                    break;
            }
        }

        private async Task LogConfessionAsync(Embed embed, SocketGuild guild, ulong loggingChannelId)
        {
            var logtext = $"By ||{Context.User.Username}#{Context.User.Discriminator} ({Context.User.Id})|| at UTC {DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture)}";

            var loggingChannel = guild.GetTextChannel(loggingChannelId);
            await loggingChannel.SendMessageAsync(logtext, embed: embed);
        }

        private async Task PublishInAllMutualGuildsAsync(IReadOnlyCollection<SocketGuild> mutualGuilds, string content, ConfessionType type)
        {
            var embed = ConfessionModule.CreateConfessionEmbed(content, type);
            foreach (SocketGuild guild in mutualGuilds)
            {
                await this.ConfessToGuildAsync(embed, guild, type);
            }

            await this.ReplyAsync(confessionbot_strings.ConfessionBotConfessionSent);
        }

        private async Task PublishInSpecificGuildAsync(IReadOnlyCollection<SocketGuild> mutualGuilds, string content, ConfessionType type)
        {
            var guilds = ConfessionModule.ToGuildArray(mutualGuilds);
            var guildIndex = await this.AttemptToGetSpecificGuildAsync(guilds);
            if (ConfessionModule.WithinArrayBounds(guilds, guildIndex))
            {
                await this.ConfessToGuildAsync(content, type, guilds[guildIndex]);
                await this.ReplyAsync(confessionbot_strings.ConfessionBotConfessionSent);
            }
            else
            {
                await this.ReplyAsync(confessionbot_strings.ConfessionBotConfessionFailed);
            }
        }

        private async Task SendServerListAsync(SocketGuild[] guilds)
        {
            await this.ReplyAsync(ConfessionModule.ServerListText(guilds));
        }

        private async Task TryToSendConfessionAsync(string content, ConfessionType type)
        {
            var mutualGuilds = this.Context.User.MutualGuilds;
            if (mutualGuilds.Count > 1)
            {
                await this.HandleMutualGuildsAmbiguityAsync(content, mutualGuilds, type);
            }
            else if (mutualGuilds.Count == 1)
            {
                await this.PublishInAllMutualGuildsAsync(mutualGuilds, content, type);
            }
            else
            {
                await this.ReplyAsync(confessionbot_strings.ConfessionBotNoMutualServer);
            }
        }
    }
}

#pragma warning restore RCS1090 // Add call to 'ConfigureAwait' (or vice versa).