using Discord;
using Discord.WebSocket;
using System;
using System.Threading;
using System.Threading.Tasks;

using bot.Services;

namespace bot
{
    class ExaltedSage
    {
        private readonly DiscordSocketClient _client;
        private readonly MessageService _messageService;
        private readonly LogService _logService;

        public ExaltedSage()
        {
            _client = new DiscordSocketClient();
            _messageService = new MessageService(_client);
            _logService = new LogService(_client);

            _client.Log += _logService.LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += _messageService.MessageReceivedAsync;
        }

        public async Task MainAsync(string token)
        {
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            // Block program until it's closed
            await Task.Delay(Timeout.Infinite);
        }

        // The Ready event indicates that the client has opened a
        // connection and it is now safe to access the cache
        private Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");

            return Task.CompletedTask;
        }
    }
}
