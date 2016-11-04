namespace StrategyIncubator
{
    class Config
    {
        public string rss { get; set; } = "";
        public string appname { get; set; } = "";
        public string game { get; set; } = "";
        public string token { get; set; } = "";
        public ulong channelid { get; set; } = 0;
        public int interval { get; set; } = 60000;
        public string discordmsg { get; set; } = "";
    }
}
