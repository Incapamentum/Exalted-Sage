using Discord;
using Discord.Net.WebSockets;
using Discord.Net.Udp;
using Discord.WebSocket;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bot.Config;
using Bot.Handlers;
using Bot.Helpers; 
using Bot.Services;

namespace Bot
{
    class ExaltedSage
    {
        // These are app specific
        private readonly string _token;

        // Client interfaces
        private readonly DiscordSocketClient _discordClient;
        private readonly DiscordSocketConfig _discordConfig;
        private readonly MongoClient _mongoClient;

        // Handlers
        //private readonly SlashCommandHandler _slashCommandHandler;
        private readonly MessageEventHandler _messageHandler;
        private readonly VoiceEventHandler _voiceHandler;

        // Services
        private readonly LogService _logService;

        public ExaltedSage(AppConfig appSettings)
        {
            _token = appSettings.settings.Token;
            DatabaseService.SetDatabaseName(appSettings.settings.DatabaseName);

            // Initializing member variables
            _discordConfig = GenerateConfig();
            _discordClient = new DiscordSocketClient(_discordConfig);
            _mongoClient = DatabaseService.EstablishConnection(
                appSettings.settings.ConnectionUri);

            //_slashCommandHandler = new SlashCommandHandler();
            _messageHandler = new MessageEventHandler(_discordClient, _mongoClient);
            _voiceHandler = new VoiceEventHandler(_discordClient, _mongoClient);

            _logService = new LogService(_discordClient);

            //_discordClient.SlashCommandExecuted += _slashCommandHandler.SlashCommandExecute;
            // Message events
            _discordClient.MessageReceived += _messageHandler.MessageReceivedAsync;
            _discordClient.MessageDeleted += _messageHandler.MessageDeletedAsync;

            // Voice channel events
            _discordClient.UserVoiceStateUpdated += _voiceHandler.VoiceStateChangeAsync;

            _discordClient.Log += _logService.LogAsync;

            _discordClient.Ready += ReadyAsync;
        }

        /// <summary>
        ///     Main execution point of the bot
        /// </summary>
        public async Task MainAsync()
        {
            await _discordClient.LoginAsync(TokenType.Bot, _token);
            await _discordClient.StartAsync();

            // Block program until it's closed
            await Task.Delay(Timeout.Infinite);
        }

        private Task ReadyAsync()
        {
            var period = new TimeSpan(1, 0, 0);
            PeriodicAsync(period);

            Console.WriteLine($"{_discordClient.CurrentUser} is online!");

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Executed hourly to check if server reset has been reached (occurs at 00:00 UTC)
        ///     to run additional tasks
        /// </summary>
        private async Task OnServerReset()
        {
            ulong channelId = 0;

            if (ReleaseMode.Mode == "ProdSettings")
                channelId = await ChannelHelper.GetChannelId(_mongoClient, "Guild", "text", "bot-channel");
            else
                channelId = 720690834638372949;

            var broadcastChannel = _discordClient.GetChannel(channelId) as SocketTextChannel;
            var utcNow = DateTime.UtcNow;

            // TODO: this may require some clean-up.
            if (utcNow.Hour == 0)
            {
                // Different collections to filter results from
                var tomorrowPveDailiesId = await HttpService.GetTomorrowsPveDailiesId();
                var dailyPveAchievements = await DatabaseService.GetDailyPveAchievements(_mongoClient);
                var dailyPveWatchlist = await DatabaseService.GetDailyPveWatchlist(_mongoClient);

                List<string> dailyPveNames = new();
                List<string> upcomingPveDailies = new();

                dailyPveNames = AchievementHelper.AchievementNamesFromIds(tomorrowPveDailiesId, dailyPveAchievements);
                upcomingPveDailies = AchievementHelper.AchievementsSetToAppear(dailyPveNames, dailyPveWatchlist);

                // List is non-empty; proceed to build an embedded message
                if (upcomingPveDailies.Count > 0)
                {
                    var embed = new EmbedBuilder
                    {
                        Title = "Daily Alert",
                        Description = "Attention! A daily achievement that is being monitored will appear tomorrow!",
                        Color = 0xffee05
                    };

                    foreach (string achieveName in upcomingPveDailies)
                    {
                        var achieveId = AchievementHelper.AchievementGetIdFromName(achieveName, dailyPveAchievements);

                        // Valid ID check
                        if (achieveId != 0)
                        {
                            var desc = await HttpService.ObtainAchievementDescription(achieveId);

                            embed.AddField(achieveName, desc);
                        }
                        else
                        {
                            continue;
                        }
                    }

                    // Broadcast the embedded message
                    await broadcastChannel.SendMessageAsync(embed: embed.Build());
                }
                else
                {
                    await broadcastChannel.SendMessageAsync("Nothing to report...");
                }
            }
        }

        /// <summary>
        ///     Wrapper class that periodically calls the async task.
        /// </summary>
        /// <param name="period">
        ///     The period of time to execute the task at.
        /// </param>
        /// <returns></returns>
        private async Task PeriodicAsync(TimeSpan period)
        { 
            while (true)
            {
                await OnServerReset();
                await Task.Delay(period);
            }
        }

        private DiscordSocketConfig GenerateConfig()
        {
            // Everything will be set to their default values unless otherwise
            // set explicitly
            DiscordSocketConfig config = new DiscordSocketConfig
            {
                MessageCacheSize = 512,

                WebSocketProvider = DefaultWebSocketProvider.Instance,
                UdpSocketProvider = DefaultUdpSocketProvider.Instance
            };

            return config;
        }
    }
}
