using System;
using System.Threading;
using System.ComponentModel;
using Discord;

namespace StrategyIncubator
{
    class DiscordApp
    {
        private BackgroundWorker _bwg;
        private DiscordClient _client;
        private Channel _channel;
        private Config _config;
        private Log _log;

        public DiscordApp(Config config)
        {
            _log = new Log("Discord", "Discord.txt", 1);
            _config = config;

            _client = new DiscordClient(o =>
            {
                o.AppName = _config.appname;
                o.AppUrl = "https://github.com/Ezzpify/StrategyIncubator";
                o.AppVersion = _config.appversion;
            });
            _client.MessageReceived += _client_MessageReceived;
            
            _bwg = new BackgroundWorker();
            _bwg.DoWork += _bwg_DoWork;
            _bwg.RunWorkerCompleted += _bwg_RunWorkerCompleted;
            _bwg.RunWorkerAsync();

            while (_channel == null)
                Thread.Sleep(500);
        }

        private void _client_MessageReceived(object sender, MessageEventArgs e)
        {
            /*Messages that this bot posts also triggers MessageReceived,
            so for that we need to filter these messages out to avoid
            processing our own messages*/
            if (e.User.Id == _client.CurrentUser.Id)
                return;

            if (_channel == null)
            {
                /*To initialize the bot account it needs to see a message.
                I don't know why, but simply typing in any channel or server
                that the bot is in (private message included) allows the bot
                to find channels among the servers it's connected to.
                
                So after a message has been seen by the bot we'll find our
                posting channel using the Channel ID provided in the Settings.*/
                _channel = _client.GetChannel(_config.channelid);

                if (_channel == null)
                    _log.Write(Log.LogLevel.Error, "Error finding channel!");
                else
                    _log.Write(Log.LogLevel.Success, "Got the channel!");
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
            /*_config.discordmsg string should include {0}, {1}, {2} & {3}
            for formatting the message here*/
            string msgStr = string.Format(_config.discordmsg, 
                post.author, 
                post.title, 
                post.link, 
                post.summary);

            try
            {
                _channel.SendMessage(msgStr);
                _log.Write(Log.LogLevel.Info, $"Alerted about new post at {post.link}");
            }
            catch (Exception ex)
            {
                _log.Write(Log.LogLevel.Error, $"Unable to send message to channel.\n{msgStr}\n{ex.Message}");
            }
        }

        public bool IsConnected()
        {
            return _client.State == ConnectionState.Connected;
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }
    }
}
