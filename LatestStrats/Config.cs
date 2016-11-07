using System.Collections.Generic;
using Newtonsoft.Json;

namespace StrategyIncubator
{
    public class Config
    {
        public int intervalMinutes { get; set; } = 2;

        public string appname { get; set; } = string.Empty;

        public string game { get; set; } = string.Empty;

        public string token { get; set; } = string.Empty;

        public List<Task> tasks { get; set; } = new List<Task>();

        [JsonIgnore]
        public string appversion { get; set; } = string.Empty;

        public bool HasMissingProperties()
        {
            return intervalMinutes == 0
                && string.IsNullOrWhiteSpace(appname)
                && string.IsNullOrWhiteSpace(game)
                && string.IsNullOrWhiteSpace(token)
                && tasks.Count == 0;
        }
    }

    public class Task
    {
        public string rss { get; set; } = string.Empty;

        public ulong channelid { get; set; } = 0;

        public string discordmsg { get; set; } = string.Empty;

        public bool HasMissingProperties()
        {
            return string.IsNullOrWhiteSpace(rss)
                && string.IsNullOrWhiteSpace(discordmsg)
                && channelid == 0;
        }
    }
}
