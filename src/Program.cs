namespace ConfessionBot
{
    using System;
    using Discord.Addons.Interactive;
    using Microsoft.Extensions.Logging;
    using SimpleDiscordBot;

    public class Program
    {
        private SimpleBot bot;

        public static void Main()
        {
            var program = new Program();
            program.Init();
            program.Run();
        }

        private void Init()
        {
            this.InitSimpleBot();
        }

        private void InitSimpleBot()
        {
            this.bot = new SimpleBot
            {
#if TRACE
                LogLevel = LogLevel.Trace,
#elif DEBUG
                LogLevel = LogLevel.Trace,
#elif RELEASE
                LogLevel = LogLevel.Debug,
#elif PUBLISH
                LogLevel = LogLevel.Information,
#endif
                InteractiveServiceConfig = new InteractiveServiceConfig
                {
                    DefaultTimeout = TimeSpan.FromHours(1)
                }
            };
            this.bot.Init();
        }

        private void Run() => this.bot.Start();
    }
}
