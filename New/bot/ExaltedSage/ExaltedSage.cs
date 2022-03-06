using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using bot.Config;
using bot.Handlers;
using bot.Services;

namespace bot
{
    class ExaltedSage
    {
        // These are app specific
        private readonly Dictionary<string, ulong> _guilds;
        private readonly string _token;

        // Client interface
        private readonly DiscordSocketClient _client;

        // Handlers
        private readonly SlashCommandHandler _slashCommandHandler;

        // Services
        private readonly MessageService _messageService;
        private readonly LogService _logService;

        public ExaltedSage(AppSettings settings)
        {
            _guilds = settings.Guilds;
            _token = settings.Token;

            _client = new DiscordSocketClient();
            _slashCommandHandler = new SlashCommandHandler();
            _messageService = new MessageService(_client, _guilds.Values.ToList());
            _logService = new LogService(_client);

            _client.SlashCommandExecuted += _slashCommandHandler.SlashCommandExecute;
            _client.MessageReceived += _messageService.MessageReceivedAsync;
            _client.Log += _logService.LogAsync;
            _client.Ready += ReadyAsync;
        }

        /// <summary>
        ///     Main execution point of the bot
        /// </summary>
        /// <returns></returns>
        public async Task MainAsync()
        {
            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();

            // Block program until it's closed
            await Task.Delay(Timeout.Infinite);
        }

        /// <summary>
        ///     Performs the building of different guild-facing commands
        /// </summary>
        /// <returns>Nothing</returns>
        private async Task ReadyAsync()
        {
            await GuildMessageRegisterCommandAsync();

            Console.WriteLine($"{_client.CurrentUser} is online!");
        }

        /// <summary>
        ///     Builds a guild-facing slash command that registers
        ///     a message by the given ID
        /// </summary>
        /// <returns>Nothing</returns>
        private async Task GuildMessageRegisterCommandAsync()
        {
            var guild = _client.GetGuild(_guilds["Bot Test Server"]);

            var guildMessageRegisterCommand = new SlashCommandBuilder()
                .WithName("register-message")
                .WithDescription("Monitors a message with given ID")
                .AddOption("message-id",
                    ApplicationCommandOptionType.String,
                    "The message ID to monitor",
                    isRequired: true);

            // This might have to be cleaned up
            try
            {
                await guild.CreateApplicationCommandAsync(guildMessageRegisterCommand.Build());
            }
            catch (HttpException e)
            {
                var json = JsonConvert.SerializeObject(e.Errors, Formatting.Indented);

                Console.WriteLine(json);
            }
        }
    }
}
