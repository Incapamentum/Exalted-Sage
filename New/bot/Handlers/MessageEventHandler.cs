using Discord;
using Discord.WebSocket;
using NLog;
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
    ///     Handles any message-related events.
    /// </summary>
    public class MessageEventHandler
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly MongoClient _mongoClient;

        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

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
        public MessageEventHandler(DiscordSocketClient discordClient, MongoClient mongoClient)
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
            var selfId = _discordClient.CurrentUser.Id;

            // Bot shouldn't process any messages it sends
            if (message.Author.Id == selfId)
                return;

            Logger.Info("Message received! Channel origin: " + message.Channel.Name);

            // Debugging purposes
            if (ReleaseMode.Mode == "ProdSettings")
            {
                // Cast to TextChannel before category ID can be obtained
                var chan = message.Channel as SocketTextChannel;
                var catId = chan.Category.Id;

                // Invokes the correct method based on category origin
                switch (catId)
                {
                    // Guild category
                    case 716050945615855777:
                        await ProcessMessageResponse(message, selfId);
                        break;

                    default:
                        break;
                }
            }
            else
            {
                // Need to test something? Put it in here.
                await ProcessMessageResponse(message, selfId);
            }
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
            // Make sure caches are valid
            if (!message.HasValue && !channel.HasValue)
                return;

            var msg = message.Value as SocketMessage;

            // Debugging purposes
            if (ReleaseMode.Mode == "ProdSettings")
            {
                var broadcastId = await DatabaseService.GetAdminAlertChannel(_mongoClient);
                var broadcastChannel = _discordClient.GetChannel(broadcastId)
                    as SocketTextChannel;

                // Cast to TextChannel before category ID can be obtained
                var chan = channel.Value as SocketTextChannel;
                var catId = chan.Category.Id;

                // Invokes the correct method based on category origin
                switch (catId)
                {
                    // Tradeing House
                    case 716046889442738178:
                        await ProcessTradeMessageDeletion(msg, broadcastChannel);
                        break;

                    default:
                        break;
                }
            }
            else
            {
                var broadcastChannel = _discordClient.GetChannel(1013185367924547654)
                    as SocketTextChannel;
                // Need to test something? Put it in here.
            }
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

            var chanId = message.Channel.Id;

            var approvedChannels = await DatabaseService.GetCategoryTextChannels(_mongoClient, "Guild");
            var approvedChannelsId = approvedChannels.Values.ToList();

            if (!approvedChannelsId.Contains(chanId) && ReleaseMode.Mode == "ProdSettings")
                return;

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
        //private static async Task ProcessMessageDeletion(SocketMessage msg, SocketTextChannel broadcast,
        //                                                 List<ulong> supervisedList)
        //{
        //    // Obtain information regarding the message deletion
        //    var user = msg.Author;
        //    var content = msg.Content;
        //    var channel = msg.Channel;

        //    var embed = new EmbedBuilder
        //    {
        //        Description = $"A message in <#{channel.Id}> has been deleted.",
        //        Color = 0xffc805
        //    };

        //    embed.AddField("Author", user);
        //    embed.AddField("Message", content);

        //    if (TradeChannelOrigin(supervisedList, channel.Id))
        //    {
        //        embed.WithTitle("Trade Message Deleted");
        //    }
        //    else
        //    {
        //        embed.WithTitle("Message Deleted");
        //    }

        //    await broadcast.SendMessageAsync(embed: embed.Build());
        //}

        /// <summary>
        ///     Processes a message deleted in a trade channel by building an embed
        ///     object to then build -> broadcast to an alerts channel.
        /// </summary>
        /// <param name="msg">
        ///     The deleted message to process.
        /// </param>
        /// <param name="broadcast">
        ///     The channel to broadcast the embed object to.
        /// </param>
        /// <returns>
        ///     None.
        /// </returns>
        private static async Task ProcessTradeMessageDeletion(SocketMessage msg, SocketTextChannel broadcast)
        {
            // Obtain information regarding the message deletion
            var user = msg.Author;
            var content = msg.Content;
            var channel = msg.Channel;

            var embed = new EmbedBuilder
            {
                Description = $"A message in <#{channel.Id}> has been deleted.",
                Color = 0xffc805
            };

            embed.AddField("Author", user);
            embed.AddField("Message", content);

            embed.WithTitle("Trade Message Deleted");

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
