using Newtonsoft.Json;
using System.Collections.Generic;

namespace StrategyIncubator
{
    public class Config
    {
        public int intervalMinutes { get; set; } = 2;
        public string appname { get; set; } = "";
        public string game { get; set; } = "";
        public string token { get; set; } = "";
        public List<Task> tasks { get; set; } = new List<Task>();

        [JsonIgnore]
        public string appversion { get; set; } = "";
    }

    public class Task
    {
        public string rss { get; set; } = "";
        public ulong channelid { get; set; } = 0;
        public string discordmsg { get; set; } = "";
    }
}
