using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Discord;
using Discord.Commands;

namespace StrategyIncubator
{
    class DiscordApp
    {
        private BackgroundWorker _bwg;
        private DiscordClient _client;
        private bool _initialized;
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

            while (!_initialized)
                Thread.Sleep(500);
        }

        public void SendMessage(Post post, Task task)
        {
            /*discordmsg string should include {0}, {1}, {2} & {3}
            for formatting the message here*/
            string msgStr = string.Format(task.discordmsg,
                post.author,
                post.title,
                post.link,
                post.summary);

            try
            {
                var channel = _client.GetChannel(task.channelid);
                if (channel == null)
                {
                    _log.Write(Log.LogLevel.Error, $"Unable to find channel to send message {msgStr}");
                    return;
                }

                channel.SendMessage(msgStr);
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

        private void _client_MessageReceived(object sender, MessageEventArgs e)
        {
            /*Messages that this bot posts also triggers MessageReceived,
            so for that we need to filter these messages out to avoid
            processing our own messages*/
            if (e.User.Id == _client.CurrentUser.Id)
                return;

            /*We'll need to receive a message first in order to find channels
            for some reason, so this is what we will do.*/
            if (!_initialized)
            {
                _log.Write(Log.LogLevel.Success, $"All done! Tasks started.");
                _initialized = true;
                registerCommands();
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

        private void registerCommands()
        {
            var cmd = new CommandService(new CommandServiceConfigBuilder
            {
                PrefixChar = '!',
                HelpMode = HelpMode.Disabled
            });

            cmd.CreateCommand("roll")
                .Description("Rolls a random number.")
                .Parameter("x", ParameterType.Required)
                .Do(ex => { OnCommandRoll(ex); });

            cmd.CreateCommand("seen")
                .Description("Returns information about the last time bot saw the user in the server.")
                .Parameter("user", ParameterType.Required)
                .Do(ex => { OnCommandSeen(ex); });

            _client.AddService(cmd);
        }

        private void OnCommandRoll(CommandEventArgs e)
        {
            int maxRoll = 0;
            int.TryParse(e.Args[0], out maxRoll);
            
            if (maxRoll > 0)
            {
                var rand = Utils.Random.GetThreadRandom();
                int result = rand.Next(1, (maxRoll + 1));

                e.Channel.SendMessage($"{e.User.Name} rolled {result}!");
            }
        }

        private void OnCommandSeen(CommandEventArgs e)
        {
            string userInput = e.Args[0];
            User user = null;

            /*If argument is a mention it will look like this
            <@123123> (numbers being userId, so we need to parse)*/
            if (userInput.Contains("@"))
            {
                userInput = Functions.GetNumbersFromString(userInput);
                if (userInput.Length > 0)
                {
                    ulong userId = Convert.ToUInt64(userInput);
                    user = e.Server.GetUser(userId);
                }
            }
            else
            {
                /*If username typed manually we can just find by username*/
                user = e.Server.FindUsers(userInput, true).LastOrDefault();
            }

            if (user != null)
            {
                bool noRecord = false;
                switch (user.Status.Value)
                {
                    case "offline":
                        if (user.LastOnlineAt.HasValue)
                            e.Channel.SendMessage($"{user.Name} was last seen online {user.LastOnlineAt.Value.ToLongTimeString()} GMT");
                        else noRecord = true;
                        break;

                    default:
                        if (user.LastActivityAt.HasValue)
                            e.Channel.SendMessage($"{user.Name} was last seen active at {user.LastActivityAt.Value.ToLongTimeString()} GMT");
                        else noRecord = true;
                        break;
                }

                if (noRecord)
                    e.Channel.SendMessage("No records available for that user.");
            }
        }
    }
}
