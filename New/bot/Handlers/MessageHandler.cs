using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;

using Bot.Services;

namespace Bot.Handlers
{
    /// <summary>
    ///     Handler class that parses messages
    /// </summary>
    
    // This will have to be overhauled:
    //      [ ] Origin of guild should no longer be tracked due to bot being
    //          independent of that
    //      [ ] It may need access to the database to pull phrases/sentences from
    //      [ ] Unsure if the bot will ever crash upon receiving a message, but be
    //          mindful of such behavior in the end
    public class MessageHandler
    {
        private readonly DiscordSocketClient _client;

        public MessageHandler(DiscordSocketClient client)
        {
            _client = client;
        }

        public async Task MessageReceivedAsync(SocketMessage message)
        {
            // Bot shouldn't process any messages it sends
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            var originChannel = message.Channel as SocketGuildChannel;
            var originGuild = originChannel.Guild.Id;

            // Ignore all messages that are not from the list of approved guilds
            //if (_guildIds.Contains(originGuild))
            //{
            //    // This should contain stuff for shitposting purposes, probably
            //    await message.Channel.SendMessageAsync("Message ID: " + message.Id.ToString());
            //}
        }
    }
}
