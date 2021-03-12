namespace ConfessionBot.Module
{
    using System.Threading.Tasks;
    using Discord;
    using Discord.Commands;
    using SimpleDiscordBot.Module;

    public class PrefixModule : SimpleBotBaseTopLevelModule
    {
        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("setconfessionprefix")]
        [Summary("Sets the server unique configuration prefix. (Requires Admin or Bot Owner)")]
        public async Task SetBotPrefixAsync([Remainder] string prefix)
            => await this.SetPrefixAsync(prefix).ConfigureAwait(true);

        [RequireOwner]
        [Command("setconfessionprefix")]
        public async Task SetBotPrefixOwnerAsync([Remainder] string prefix)
            => await this.SetPrefixAsync(prefix).ConfigureAwait(true);

        private async Task SetPrefixAsync(string prefix = "!")
        {
            this.GetServerStorage(Context.Guild.Id).Prefix = prefix;
            await this.ReplyAsync("Set the configuration prefix: " + prefix).ConfigureAwait(true);
        }
    }
}
