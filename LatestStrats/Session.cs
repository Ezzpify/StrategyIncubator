using System.Linq;
using System.Threading;
using System.ServiceModel.Syndication;
using System.Xml;

namespace LatestStrats
{
    class Session
    {
        private DiscordCom _discord;
        private Database _database;
        private Timer _queryTimer;
        private Config _config;
        private Log _log;

        public Session(Config config)
        {
            _log = new Log("Session", "Session.txt", 1);
            _database = new Database("Database.sqlite");
            _config = config;

            if (_config == null)
            {
                _log.Write(Log.LogLevel.Error, $"Config is null, cannot continue");
                Thread.Sleep(1500);
                return;
            }

            _discord = new DiscordCom(_config);
            _queryTimer = new Timer(TimerCallback, null, 0, _config.interval);
            
            while (true)
                Thread.Sleep(500);
        }

        void TimerCallback(object o)
        {
            XmlReader reader = XmlReader.Create(_config.rss);
            SyndicationFeed feed = SyndicationFeed.Load(reader);

            foreach (var item in feed.Items.Where(x => x != null).Reverse())
            {
                var post = new Post()
                {
                    link = item.Id,
                    summary = item.Summary.Text,
                    author = item.ElementExtensions.ReadElementExtensions<string>("creator", "http://purl.org/dc/elements/1.1/")[0]
                };

                if (!post.Validate())
                    continue;

                long unix = Functions.ConvertToUnixTime(item.PublishDate.DateTime);
                if (_database.DoesUnixExist(unix))
                    continue;

                _discord.SendMessage(post);
                _database.InsertUnix(unix);
                _log.Write(Log.LogLevel.Info, $"Alerted about new post at {post.link}");
            }
        }
    }
}
