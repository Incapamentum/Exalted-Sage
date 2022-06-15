using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;

using Bot.Services;

namespace Bot.Handlers
{
    /// <summary>
    ///     Handler class that parses message contents
    /// </summary>
    public class MessageHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly List<ulong> _guildIds;

        public MessageHandler(DiscordSocketClient client, List<ulong> guildIds)
        {
            _client = client;
            _guildIds = guildIds;
        }

        public async Task MessageReceivedAsync(SocketMessage message)
        {
            // Bot shouldn't process any messages it sends
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            var originChannel = message.Channel as SocketGuildChannel;
            var originGuild = originChannel.Guild.Id;

            // Ignore all messages that are not from the list of approved guilds
            if (_guildIds.Contains(originGuild))
            {
                // This should contain stuff for shitposting purposes, probably
                await message.Channel.SendMessageAsync("Message ID: " + message.Id.ToString());
            }
        }
    }
}
