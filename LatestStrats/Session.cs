using System;
using System.Linq;
using System.Threading;
using System.ServiceModel.Syndication;
using System.Xml;

namespace StrategyIncubator
{
    class Session
    {
        private DiscordApp _discord;
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
                _log.Write(Log.LogLevel.Error, $"Config is null, cannot continue. Press any key to exit.");
                Console.ReadKey();
                return;
            }

            _discord = new DiscordApp(_config);
            _queryTimer = new Timer(queryTimerCallback, null, 0,
                Functions.ConvertMinutesToMilliseconds(_config.intervalMinutes));

            while (_discord.IsConnected())
                Thread.Sleep(5000);

            _log.Write(Log.LogLevel.Error, $"Discord disconnected. Ending session...");

            _queryTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _database.CloseConnection();
            _discord.Disconnect();
        } 

        private void queryTimerCallback(object o)
        {
            foreach (var task in _config.tasks)
            {
                SyndicationFeed feed;

                try
                {
                    XmlReader reader = XmlReader.Create(task.rss);
                    feed = SyndicationFeed.Load(reader);
                }
                catch (Exception ex)
                {
                    _log.Write(Log.LogLevel.Error, $"Error parsing xml. {ex}");
                    return;
                }

                /*We reverse the list since newest post is always at the top,
                but if two posts are made after eachother we want to alert about
                the oldest one first, so we reverse the list.*/
                foreach (var item in feed.Items.Where(x => x != null).Reverse())
                {
                    long unix = Functions.ConvertToUnixTime(item.PublishDate.DateTime);
                    if (_database.DoesUnixExist(unix))
                        continue;

                    Post post = GetPostFromXmlItem(item);
                    if (post.Validate())
                    {
                        _discord.SendMessage(post, task);
                        _database.InsertUnix(unix);
                    }
                }
            }
        }

        private Post GetPostFromXmlItem(SyndicationItem item)
        {
            var post = new Post()
            {
                title = Functions.NullcheckStr(item.Title.Text),
                link = Functions.NullcheckStr(item.Id),
                summary = Functions.DiscordifyString
                    (Functions.NullcheckStr(item.Summary.Text))
            };

            var elements = item.ElementExtensions
                    .ReadElementExtensions<string>("creator", "http://purl.org/dc/elements/1.1/");

            /*Index 0 is the main author name in cd:creator*/
            post.author = elements.Count() > 0 ? elements[0] : "Unknown";
            return post;
        }
    }
}
