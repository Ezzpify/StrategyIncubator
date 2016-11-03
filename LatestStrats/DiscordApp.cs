using System.Collections.Generic;
using System.Threading;
using Discord;
using System.ComponentModel;

namespace StrategyIncubator
{
    class DiscordApp
    {
        private List<ulong> _ignoredusers;
        private DiscordClient _client;
        private BackgroundWorker _bwg;
        private Channel _channel;
        private bool _connected;
        private Config _config;
        private Log _log;

        public DiscordApp(Config config)
        {
            _log = new Log("Discord", "Discord.txt", 1);
            _config = config;

            _client = new DiscordClient(o =>
            {
                o.AppName = _config.appname;
            });
            _client.MessageReceived += _client_MessageReceived;

            _bwg = new BackgroundWorker();
            _bwg.DoWork += _bwg_DoWork;
            _bwg.RunWorkerCompleted += _bwg_RunWorkerCompleted;
            _bwg.RunWorkerAsync();

            while (!_connected)
                Thread.Sleep(500);
        }

        private void _client_MessageReceived(object sender, MessageEventArgs e)
        {
            if (e.User.Id == _client.CurrentUser.Id)
                return;

            if (_channel == null)
            {
                _channel = _client.GetChannel(_config.channelid);
                if (_channel == null)
                    _log.Write(Log.LogLevel.Error, "Error finding channel!");
                else
                {
                    _log.Write(Log.LogLevel.Success, "Got the channel!");
                    _connected = true;
                }

                return;
            }

            if (e.Channel.IsPrivate && !_ignoredusers.Contains(e.User.Id))
            {
                e.User.SendMessage("I don't have anything in particular to say. But you can find my source code here: https://github.com/Ezzpify/StrategyIncubator.");
                _ignoredusers.Add(e.User.Id);
            }
        }

        private void _bwg_DoWork(object sender, DoWorkEventArgs e)
        {
            _client.ExecuteAndWait(async () =>
            {
                await _client.Connect(_config.token, TokenType.Bot);
                _client.SetGame(_config.game);
                _log.Write(Log.LogLevel.Info, "Waiting for user to type a message to initialize...", false);
            });
        }

        private void _bwg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                _log.Write(Log.LogLevel.Error, $"An uncaught exception killed the background worker: {e.Error}");

            _log.Write(Log.LogLevel.Info, $"Backgroundworker exited.");
        }

        public void SendMessage(Post post)
        {
            string formattedStr = $"*New strategy post by* ***{post.author}*** *to thread* ***{post.title}***"
                + $"\n<{post.link}>"
                + $"\n--------------------------------------------"
                + $"\n{post.summary}";

            _channel.SendMessage(formattedStr);
        }
    }
}
