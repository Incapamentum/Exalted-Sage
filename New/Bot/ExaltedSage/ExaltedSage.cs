using Discord;
using Discord.Net.WebSockets;
using Discord.Net.Udp;
using Discord.WebSocket;
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
        // Token to authenticate the bot to Discord
        private readonly string _token;

        // Dyanmic services
        private readonly DatabaseService _databaseService;

        // Discord-specific client and configuration
        private readonly DiscordSocketClient _discordClient;
        private readonly DiscordSocketConfig _discordConfig;

        // Handlers
        //private readonly SlashCommandHandler _slashCommandHandler;
        private readonly GuildMemberEventHandler _guildMemberEventHandler;
        private readonly MessageEventHandler _messageHandler;
        private readonly UserEventHandler _userHandler;
        private readonly VoiceEventHandler _voiceHandler;

        // Services
        private readonly LogService _logService;

        public ExaltedSage(AppConfig appSettings)
        {
            _token = appSettings.settings.Token;

            // Initializing the Discord client interface
            _discordConfig = GenerateConfig();
            _discordClient = new DiscordSocketClient(_discordConfig);

            // Establishing connection to the database
            _databaseService = new DatabaseService(
                appSettings.settings.DatabaseName,
                appSettings.settings.ConnectionUri);

            //_slashCommandHandler = new SlashCommandHandler();
            // Initializing event handlers
            _guildMemberEventHandler = new GuildMemberEventHandler
                (_discordClient, _databaseService);
            _messageHandler = new MessageEventHandler(_discordClient,
                _databaseService);
            _userHandler = new UserEventHandler(_discordClient,
                _databaseService);
            _voiceHandler = new VoiceEventHandler(_discordClient,
                _databaseService);

            _logService = new LogService(_discordClient);

            // Hooking up each event handler to its corresponding listener
            // through the client
            _discordClient.GuildMemberUpdated 
                += _guildMemberEventHandler.GuildMemberUpdatedAsync;
            _discordClient.MessageReceived
                += _messageHandler.MessageReceivedAsync;
            _discordClient.MessageDeleted
                += _messageHandler.MessageDeletedAsync;
            _discordClient.UserLeft += _userHandler.UserLeftAsync;
            _discordClient.UserVoiceStateUpdated 
                += _voiceHandler.VoiceStateChangeAsync;

            // Adding the log service to post status message when ready
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

            PeriodicAsync(OnServerReset, period);

            Console.WriteLine($"{_discordClient.CurrentUser} is online!");

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Executed hourly to check if server reset has been reached
        ///     (occurs at 00:00 UTC) to run additional tasks
        /// </summary>
        private async Task OnServerReset()
        {
            ulong channelId = 0;

            if (ReleaseMode.Mode == "Prod")
                channelId = await ChannelHelper.GetChannelId(_databaseService,
                    "Guild", "text", "bot-channel");
            else
                channelId = 720690834638372949;

            var broadcastChannel = _discordClient.GetChannel(channelId)
                as SocketTextChannel;
            var utcNow = DateTime.UtcNow;

            // TODO: this may require some clean-up.
            if (utcNow.Hour == 0)
            {
                // Different collections to filter results from
                var tomorrowPveDailiesId = await HttpService
                    .GetTomorrowsPveDailiesId();
                var dailyPveAchievements = await _databaseService
                    .GetDailyPveAchievements();
                var dailyPveWatchlist = await _databaseService
                    .GetDailyPveWatchlist();

                List<string> dailyPveNames = new();
                List<string> upcomingPveDailies = new();

                dailyPveNames = AchievementHelper
                    .AchievementNamesFromIds(tomorrowPveDailiesId,
                    dailyPveAchievements);
                upcomingPveDailies = AchievementHelper
                    .AchievementsSetToAppear(dailyPveNames, dailyPveWatchlist);

                // List is non-empty; proceed to build an embedded message
                if (upcomingPveDailies.Count > 0)
                {
                    var embed = new EmbedBuilder
                    {
                        Title = "Daily Alert",
                        Description = "Attention! A daily achievement that is" +
                        " being monitored will appear tomorrow!",
                        Color = 0xffee05
                    };

                    foreach (string achieveName in upcomingPveDailies)
                    {
                        var achieveId = AchievementHelper
                            .AchievementGetIdFromName(achieveName,
                            dailyPveAchievements);

                        // Valid ID check
                        if (achieveId != 0)
                        {
                            var desc = await HttpService
                                .ObtainAchievementDescription(achieveId);

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
        /// <param name="action">
        ///     The async task to call
        /// </param>
        /// <param name="interval">
        ///     The time period to invoke the action
        /// </param>
        /// <param name="cancellationToken"></param>
        /// 
        private static async Task PeriodicAsync(Func<Task> action,
            TimeSpan interval, CancellationToken cancellationToken = default)
        {
            using var timer = new PeriodicTimer(interval);
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                await action();
            }
        }

        private DiscordSocketConfig GenerateConfig()
        {
            // Everything will be set to their default values unless otherwise
            // set explicitly
            DiscordSocketConfig config = new DiscordSocketConfig
            {
                MessageCacheSize = 512,
                AlwaysDownloadUsers= true,

                GatewayIntents = GatewayIntents.AllUnprivileged 
                    | GatewayIntents.GuildMembers
                    | GatewayIntents.MessageContent,

                WebSocketProvider = DefaultWebSocketProvider.Instance,
                UdpSocketProvider = DefaultUdpSocketProvider.Instance
            };

            return config;
        }
    }
}
