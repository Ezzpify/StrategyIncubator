namespace StrategyIncubator
{
    class Config
    {
        public string rss { get; set; } = "";
        public string appname { get; set; } = "";
        public string game { get; set; } = "";
        public string clientid { get; set; } = "";
        public string secret { get; set; } = "";
        public string token { get; set; } = "";
        public ulong channelid { get; set; } = 0;
        public int interval { get; set; } = 60000;
    }
}
