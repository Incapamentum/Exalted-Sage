using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace bot
{
    class ExaltedSage
    {
        private readonly DiscordSocketClient _client;

        static void Main(string[] args)
        {
            new ExaltedSage().MainAsync().GetAwaiter().GetResult();
        }

        public ExaltedSage()
        {
            _client = new DiscordSocketClient();

            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;
        }

        public async Task MainAsync()
        {
            // Tokens should be considered secret data, and never hard-coded
            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("token"));
            await _client.StartAsync();

            // Block program until it's closed
            await Task.Delay(Timeout.Infinite);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        // Ready event indicates that the client has opened a
        // connection and it is now safe to access the cache
        private Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");

            return Task.CompletedTask;
        }

        // Not recommended way to write a bot - consider
        // reading over the Commands Framework sample
        private async Task MessageReceivedAsync(SocketMessage message)
        {
            // Bot should never respond to itself
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            if (message.Content == "!ping")
                await message.Channel.SendMessageAsync("pong!");
        }
    }
}
