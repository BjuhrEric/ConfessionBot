namespace ConfessionBot.Module
{
    using System.Threading.Tasks;
    using Discord;
    using Discord.Commands;
    using SimpleDiscordBot.Module;

    public class ConfessionConfigModule : SimpleBotBaseTopLevelModule
    {
        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("clearconfessionlogchannel")]
        [Summary("Clears the confession logging channel. (Requires Admin)")]
        public async Task ClearConfessionLoggingChannelAsync()
            => await this.ClearLoggingChannelAsync(ConfessionType.LoveConfession).ConfigureAwait(true);

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("clearconfessionchannel")]
        [Summary("Clears the confession publishing channel. (Requires Admin)")]
        public async Task ClearConfessionPublishChannelAsync()
            => await this.ClearConfessionChannelAsync(ConfessionType.LoveConfession).ConfigureAwait(true);

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("clearventinglogchannel")]
        [Summary("Clears the vent logging channel. (Requires Admin)")]
        public async Task ClearVentLoggingChannelAsync()
            => await this.ClearLoggingChannelAsync(ConfessionType.VentConfession).ConfigureAwait(true);

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("clearventingchannel")]
        [Summary("Clears the vent publishing channel. (Requires Admin)")]
        public async Task ClearVentPublishChannelAsync()
            => await this.ClearConfessionChannelAsync(ConfessionType.VentConfession).ConfigureAwait(true);

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("setconfessionchannel")]
        [Summary("Sets the publishing channel for confessions to either the current one or a given one. (Requires Admin)")]
        public async Task MakeConfessionChannelAsync([Remainder] ITextChannel channel = null)
            => await this.SetConfessionChannelAsync(channel, ConfessionType.LoveConfession).ConfigureAwait(true);

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("setconfessionlogchannel")]
        [Summary("Sets the logging channel for confessions to either the current one or a given one. (Requires Admin)")]
        public async Task MakeConfessionLoggingChannelAsync([Remainder] ITextChannel channel = null)
            => await this.SetLoggingChannelAsync(channel, ConfessionType.LoveConfession).ConfigureAwait(true);

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("setventingchannel")]
        [Summary("Sets the publishing channel for vent messages to either the current one or a given one. (Requires Admin)")]
        public async Task MakeVentingChannelAsync([Remainder] ITextChannel channel = null)
            => await this.SetConfessionChannelAsync(channel, ConfessionType.VentConfession).ConfigureAwait(true);

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("setventinglogchannel")]
        [Summary("Sets the logging channel for vent messages to either the current one or a given one. (Requires Admin)")]
        public async Task MakeVentingLoggingChannelAsync([Remainder] ITextChannel channel = null)
            => await this.SetLoggingChannelAsync(channel, ConfessionType.VentConfession).ConfigureAwait(true);

        private async Task ClearConfessionChannelAsync(ConfessionType type)
        {
            var storage = this.GetServerStorage(Context.Guild.Id);
            storage.ClearNamedID(type.PublishChannelName);
            await this.ReplyAsync("Channel setting cleared!").ConfigureAwait(true);
        }

        private async Task ClearLoggingChannelAsync(ConfessionType type)
        {
            var storage = this.GetServerStorage(Context.Guild.Id);
            storage.ClearNamedID(type.LoggingChannelName);
            await this.ReplyAsync("Channel setting cleared!").ConfigureAwait(true);
        }

        private async Task SetConfessionChannelAsync(ITextChannel channel, ConfessionType type)
        {
            var confessionChannel = channel ?? this.Context.Channel as ITextChannel;
            var storage = this.GetServerStorage(Context.Guild.Id);
            storage.SetNamedID(type.PublishChannelName, confessionChannel.Id);
            await this.ReplyAsync("Channel updated to: " + confessionChannel.Name).ConfigureAwait(true);
        }

        private async Task SetLoggingChannelAsync(ITextChannel channel, ConfessionType type)
        {
            var loggingChannel = channel ?? Context.Channel as ITextChannel;
            var storage = this.GetServerStorage(Context.Guild.Id);
            storage.SetNamedID(type.LoggingChannelName, loggingChannel.Id);
            await this.ReplyAsync("Channel updated to: " + loggingChannel.Name).ConfigureAwait(true);
        }
    }
}
