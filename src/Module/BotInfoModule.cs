namespace ConfessionBot.Module
{
    using System.Threading.Tasks;
    using ConfessionBot.res.strings;
    using Discord;
    using Discord.Commands;
    using SimpleDiscordBot.Module;

    public class BotInfoModule : SimpleBotBaseTopLevelModule
    {
        public CommandService CommandService { get; set; }

        [RequireOwner]
        [Command("confessionbotconfiginfo")]
        [Summary("Posts configuration information about the bot to either the current channel or a given one. (Requires Admin or Bot Owner)")]
        public async Task SendBotInformationAdminAsync([Remainder] ITextChannel channel = null)
            => await this.SendBotInformationAsync(channel, true).ConfigureAwait(true);

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("confessionbotconfiginfo")]
        public async Task SendBotInformationOwnerAsync([Remainder] ITextChannel channel = null)
            => await this.SendBotInformationAsync(channel, true).ConfigureAwait(true);

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("confessionbotusageinfo")]
        [Summary("Posts usage information about the bot to either the current channel or a given one. (Requires Admin or Bot Owner)")]
        public async Task SendBotUsageInformationAdminAsync([Remainder] ITextChannel channel = null)
        => await this.SendBotInformationAsync(channel).ConfigureAwait(true);

        [RequireOwner]
        [Command("confessionbotusageinfo")]
        public async Task SendBotUsageInformationOwnerAsync([Remainder] ITextChannel channel = null)
        => await this.SendBotInformationAsync(channel).ConfigureAwait(true);

        [Command("confessionsourcecode")]
        [Summary("Sends a link to the github repository.")]
        [Remarks("DM")]
        public async Task SourceAsync()
        {
            if (!this.Context.IsPrivate)
            {
                return;
            }

            await this.Context.User.SendMessageAsync(confessionbot_strings.ConfessionBotSource).ConfigureAwait(true);
        }

        private static EmbedFieldBuilder CreateCommandField(string prefix, CommandInfo command)
        {
            var commandName = command.Remarks == null ? $"{prefix}{command.Name}" : $"(DM) {command.Name}";
            return new EmbedFieldBuilder { Name = commandName, Value = command.Summary };
        }

        private static bool ShouldIncludeCommand(bool admin, CommandInfo command)
        {
            return command.Summary != null && (admin ^ command.Preconditions.Count == 0);
        }

        private Embed CreateBotInfoEmbed(bool admin = false)
        {
            var builder = new EmbedBuilder().WithTitle(confessionbot_strings.ConfessionBotInfoTitle);
            if (!admin)
            {
                builder = builder.WithDescription(confessionbot_strings.ConfessionBotInfoDescription);
            }

            var prefix = GetServerStorage(Context.Guild.Id).Prefix;
            foreach (var command in CommandService.Commands)
            {
                if (ShouldIncludeCommand(admin, command))
                {
                    builder.AddField(CreateCommandField(prefix, command));
                }
            }

            return builder.Build();
        }

        private async Task SendBotInformationAsync(ITextChannel channel = null, bool admin = false)
        {
            var targetChannel = channel ?? Context.Channel as ITextChannel;
            await targetChannel.SendMessageAsync(embed: this.CreateBotInfoEmbed(admin)).ConfigureAwait(true);
        }
    }
}
