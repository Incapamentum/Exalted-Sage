using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace bot.Services
{
    public class LogService
    {
        private readonly DiscordSocketClient _client;

        public LogService(DiscordSocketClient client)
        {
            _client = client;
        }

        public Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }
    }
}
