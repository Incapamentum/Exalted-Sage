using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using bot.Plugins;

namespace bot.Services
{
    /// <summary>
    ///     Handles the routing of received messages depending on the content of the
    ///     message
    /// </summary>
    public class MessageService
    {
        private readonly DiscordSocketClient _client;
        private readonly List<ulong> _guildIds;
        //private readonly Dictionary<string, ulong> _guilds;
        private readonly AdminCommands _adminCommands;

        public MessageService(DiscordSocketClient client, List<ulong> guildIds)
        {
            _client = client;
            _guildIds = guildIds;
            _adminCommands = new AdminCommands(client);
        }

        // This should filter the incoming message for processing
        public async Task MessageReceivedAsync(SocketMessage message)
        {
            // We don't care if the bot sends a message
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            var originChannel = message.Channel as SocketGuildChannel;
            var originGuild = originChannel.Guild.Id;

            // Ignore all messages that are not from approved guilds
            if (_guildIds.Contains(originGuild))
            {
                // What should be done here is for shitposting purposes, probably

                //message.

                await message.Channel.SendMessageAsync("Message ID " + message.Id.ToString());
            }
        }
    }
}
