using Discord;
using Discord.WebSocket;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Bot.Config;
using Bot.Helpers;
using Bot.Services;

namespace Bot.Handlers
{
    /// <summary>
    ///     Handler class that parses messages
    /// </summary>
    public class MessageHandler
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly MongoClient _mongoClient;

        /// <summary>
        ///     Handles specific event-driven processes triggered by
        ///     messages.
        /// </summary>
        /// <param name="discordClient">
        ///     The SocketClient interface to interact with Discord.
        /// </param>
        /// <param name="mongoClient">
        ///     The MongoClient interface that accesses collections
        ///     from the database.
        /// </param>
        public MessageHandler(DiscordSocketClient discordClient, MongoClient mongoClient)
        {
            _discordClient = discordClient;
            _mongoClient = mongoClient;
        }

        /// <summary>
        ///     Triggered whenever a message is sent by a user, therefore being
        ///     received by the bot. Processing depends on the origin of the
        ///     message.
        /// </summary>
        /// <param name="message">
        ///     The received message.
        /// </param>
        /// <returns>
        ///     None.
        /// </returns>
        public async Task MessageReceivedAsync(SocketMessage message)
        {
            string response = null;
            ulong channelId;

            var selfId = _discordClient.CurrentUser.Id;

            // Bot shouldn't process any messages it sends
            if (message.Author.Id == selfId)
                return;

            // Debugging purposes
            if (ReleaseMode.Mode == "DevSettings")
                channelId = 720690834638372949;
            else
                channelId = message.Channel.Id;

            var allowableChannels = await DatabaseService.GetGeneralChannels(_mongoClient);           
            
            // Don't process the message if origin channel is not whitelisted.
            if (!allowableChannels.ContainsValue(channelId) && ReleaseMode.Mode == "ProdSettings")
                return;

            await ProcessMessageResponse(message, selfId);
        }

        /// <summary>
        ///     Currently keeps track of messages deleted only in supervised channels.
        ///     Upon message deletion, creates an embedded message to publish to the
        ///     indicated broadcast channel.
        /// </summary>
        /// <param name="message">
        ///     Cacheable copy of the deleted message. Not guaranteed to actually
        ///     be cached.
        /// </param>
        /// <param name="channel">
        ///     Cacheable copy of the channel from which the message was deleted
        ///     from. Not guaranteed to actually be cached.
        /// </param>
        /// <returns>
        ///     None.
        /// </returns>
        public async Task MessageDeletedAsync(Cacheable<IMessage, UInt64> message,
                                              Cacheable<IMessageChannel, UInt64> channel)
        {
            ulong broadcastId = 0;
            string channelName = "Unknown";

            // Make sure caches are valid
            if (!message.HasValue && !channel.HasValue)
                return;

            if (ReleaseMode.Mode == "DevSettings")
                broadcastId = 1013185367924547654;
            else
                broadcastId = await ChannelHelper.GetChannelId(_mongoClient, "broadcast", "bot-alerts");

            var broadcastChannel = _discordClient.GetChannel(broadcastId)
                as SocketTextChannel;

            // Ensure deleted message was from a supervised channel
            var supervisedCollection = await DatabaseService.GetSupervisedChannels(_mongoClient);
            var supervisedIds = supervisedCollection.Values.ToList();

            if (supervisedIds.Contains(channel.Id))
                channelName = channel.Value.Name;

            // Process
            var user = message.Value.Author;
            var content = message.Value.Content;

            var embed = new EmbedBuilder
            {
                Title = "Trade Message Deleted",
                Description = $"A message in {channelName} has been deleted.",
                Color = 0xffc805
            };

            embed.AddField("Author", user);
            embed.AddField("Message", content);

            await broadcastChannel.SendMessageAsync(embed: embed.Build());
        }

        /// <summary>
        ///     Retrieves a response doc from the database, then returns
        ///     a randomly selected response.
        /// </summary>
        /// <param name="responseType">
        ///     The title of the response doc.
        /// </param>
        /// <returns>
        ///     A randomly selected response from the doc.
        /// </returns>
        private async Task<string> RandomResponse(string responseType)
        {
            string randomResponse = null;

            var responses = await DatabaseService.GetResponses(_mongoClient, responseType);
            var index = RandomHelper.rand.Next(responses.Count);

            randomResponse = responses[index];

            return randomResponse;
        }

        /// <summary>
        ///     Process the received message by extracting data from it, which is used
        ///     in determining which response the bot will reply with.
        /// </summary>
        /// <param name="message">
        ///     The SocketMessage that the bot received.
        /// </param>
        /// <param name="botId">
        ///     The UID of the bot.
        /// </param>
        private async Task ProcessMessageResponse(SocketMessage message, ulong botId)
        {
            string response = null;

            var content = message.Content.ToLower();
            var contentStrings = content.Split(' ').ToList();
            var mentioned = new List<SocketUser>(message.MentionedUsers);

            if (BotWasPinged(mentioned, botId))
            {
                response = await RandomResponse("Gilded Responses");
            }
            else if (MessageContainsSubstring(contentStrings, "tarir"))
            {
                response = await RandomResponse("Tarir Responses");
            }
            else if (MessageContainsSubstring(contentStrings, "auric basin", "ab"))
            {
                response = await RandomResponse("Auric Basin Responses");
            }
            else if (RandomHelper.rand.NextDouble() < 0.005)
            {
                response = await RandomResponse("Grab Bag Responses");
            }

            if (response != null)
            {
                await message.Channel.SendMessageAsync(response);
            }
        }

        /// <summary>
        ///     Determines whether the bot was mentioned given a list of
        ///     users mentioned in a message.
        /// </summary>
        /// <param name="mentionedUsers">
        ///     A list of SocketUsers obtained from a message.
        /// </param>
        /// <param name="botId">
        ///     The UID of the bot.
        /// </param>
        /// <returns></returns>
        private Boolean BotWasPinged(List<SocketUser> mentionedUsers, ulong botId)
        {
            if (mentionedUsers.Count > 0)
            {
                foreach (var user in mentionedUsers)
                {
                    if (user.Id == botId)
                        return true;
                }
            }

            return false;
        }

        private Boolean MessageContainsSubstring(List<String> contentStrings, params string[] substrings)
        {
            var chance = RandomHelper.rand.NextDouble();

            foreach (var substring in substrings)
            {
                if (contentStrings.Contains(substring) && chance < 0.25)
                    return true;
            }

            return false;
        }
    }
}
