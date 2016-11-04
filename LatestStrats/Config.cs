using Newtonsoft.Json;

namespace StrategyIncubator
{
    public class Config
    {
        public string rss { get; set; } = "";
        public string appname { get; set; } = "";
        public string game { get; set; } = "";
        public string token { get; set; } = "";
        public ulong channelid { get; set; } = 0;
        public int intervalMinutes { get; set; } = 2;
        public string discordmsg { get; set; } = "";

        [JsonIgnore]
        public string appversion { get; set; } = "";
    }
}
