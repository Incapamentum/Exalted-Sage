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
        /// <remarks>
        ///     NOTE: 
        /// </remarks>
        public async Task MessageDeletedAsync(Cacheable<IMessage, UInt64> message,
                                              Cacheable<IMessageChannel, UInt64> channel)
        {
            ulong broadcastId = 0;
            //string channelName = "Unknown";

            // Make sure caches are valid
            if (!message.HasValue && !channel.HasValue)
                return;

            // Debugging purposes
            if (ReleaseMode.Mode == "DevSettings")
                broadcastId = 1013185367924547654;
            else
                broadcastId = await ChannelHelper.GetChannelId(_mongoClient, "broadcast", "bot-alerts");

            var broadcastChannel = _discordClient.GetChannel(broadcastId)
                as SocketTextChannel;

            // Alot of the below can probably be condensed into a single function
            var supervisedCollection = await DatabaseService.GetSupervisedChannels(_mongoClient);
            var supervisedIds = supervisedCollection.Values.ToList();

            // Ensure deleted message was from a supervised channel
            if (!supervisedIds.Contains(channel.Id))
                return;

            var msg = message.Value as SocketMessage;

            await ProcessMessageDeletion(msg, broadcastChannel, supervisedIds);
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
        ///     Processes the deleted message by creating an embed object to build
        ///     and post on a specific broadcast channel.
        /// </summary>
        /// <param name="msg">
        ///     The deleted message to process.
        /// </param>
        /// <param name="broadcast">
        ///     The channel to broadcast the embed object to.
        /// </param>
        /// <param name="supervisedList">
        ///     The list of channel IDs under supervision.
        /// </param>
        /// <returns>
        ///     None.
        /// </returns>
        private static async Task ProcessMessageDeletion(SocketMessage msg, SocketTextChannel broadcast,
                                                         List<ulong> supervisedList)
        {
            // Obtain information regarding the message deletion
            var user = msg.Author;
            var content = msg.Content;
            var channel = msg.Channel;

            var embed = new EmbedBuilder
            {
                Description = $"A message in #{channel.Id} has been deleted.",
                Color = 0xffc805
            };

            embed.AddField("Author", user);
            embed.AddField("Message", content);

            if (TradeChannelOrigin(supervisedList, channel.Id))
            {
                embed.WithTitle("Trade Message Deleted");
            }
            else
            {
                embed.WithTitle("Message Deleted");
            }

            await broadcast.SendMessageAsync(embed: embed.Build());
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
        private static bool BotWasPinged(List<SocketUser> mentionedUsers, ulong botId)
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

        /// <summary>
        ///     Checks to see if the passed split message contents contain any
        ///     of the passed substrings.
        /// </summary>
        /// <param name="contentStrings">
        ///     The split message contents.
        /// </param>
        /// <param name="substrings">
        ///     The list of substrings to check against.
        /// </param>
        /// <returns>
        ///     True if message contains any of the substrings, false otherwise.
        /// </returns>
        private static bool MessageContainsSubstring(List<String> contentStrings, params string[] substrings)
        {
            var chance = RandomHelper.rand.NextDouble();

            foreach (var substring in substrings)
            {
                if (contentStrings.Contains(substring) && chance < 0.25)
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Determines if the channel from which a message originated from
        ///     came from a trade channel
        /// </summary>
        /// <param name="channelIds">
        ///     The list of trade channel IDs.
        /// </param>
        /// <param name="channelId">
        ///     The ID of the channel origin for the message.
        /// </param>
        /// <returns>
        ///     True if message originated from a trade channel, false
        ///     otherwise.
        /// </returns>
        /// <remarks>
        ///     NOTE: This function could probably be further generalized to be
        ///     applicable towards any subset of channels if it ever gets that
        ///     way.
        /// </remarks>
        private static bool TradeChannelOrigin(List<ulong> channelIds, ulong channelId)
        {
            if (channelIds.Contains(channelId))
                return true;

            return false;
        }
    }
}
