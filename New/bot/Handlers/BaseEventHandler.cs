using Bot.Services;
using Discord.WebSocket;

namespace Bot.Handlers
{
    /// <summary>
    ///     Base class that all event handlers inherit from.
    /// </summary>
    public class BaseEventHandler
    {
        private protected readonly DiscordSocketClient _discordClient;
        private protected readonly DatabaseService _dbService;

        private protected BaseEventHandler(DiscordSocketClient discordClient,
        DatabaseService dbService)
        {
            _discordClient = discordClient;
            _dbService = dbService;
        }
    }
    
}
