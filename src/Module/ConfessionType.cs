namespace ConfessionBot.Module
{
    public struct ConfessionType
    {
        public const string LoveConfessionLoggingChannel = "confessionLoggingChannelID";

        public const string LoveConfessionPublishChannel = "confessionPublishChannelID";

        public const string VentConfessionLoggingChannel = "ventingLoggingChannelID";

        public const string VentConfessionPublishChannel = "ventingPublishChannelID";

        public readonly static ConfessionType LoveConfession = new ConfessionType
        {
            PublishChannelName = LoveConfessionPublishChannel,
            LoggingChannelName = LoveConfessionLoggingChannel,
            EmbedTitle = "Confession"
        };

        public readonly static ConfessionType VentConfession = new ConfessionType
        {
            PublishChannelName = VentConfessionPublishChannel,
            LoggingChannelName = VentConfessionLoggingChannel,
            EmbedTitle = "Vent"
        };

        public string EmbedTitle { get; init; }

        public string LoggingChannelName { get; init; }

        public string PublishChannelName { get; init; }
    }
}
