﻿using Discord;
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

        public MessageHandler(DiscordSocketClient discordClient, MongoClient mongoClient)
        {
            _discordClient = discordClient;
            _mongoClient = mongoClient;
        }

        public async Task MessageReceivedAsync(SocketMessage message)
        {
            string response = null;
            ulong channelId;

            var allowableChannels = await DatabaseService.GetGeneralChannels(_mongoClient);

            var selfId = _discordClient.CurrentUser.Id;

            // Debugging purposes
            if (ReleaseMode.Mode == "DevSettings")
                channelId = 720690834638372949;
            else
                channelId = message.Channel.Id;

            // Bot shouldn't process any messages it sends
            if (message.Author.Id == selfId)
                return;

            // In production, bot is only allowed to send messages
            // in the allowable list
            if (!allowableChannels.ContainsValue(channelId) && ReleaseMode.Mode == "ProdSettings")
                return;

            // Grabbing the actual message then converting to a list of strings.
            // Also grabbing a list of mentioned users. Null if none are mentioned.
            var content = message.Content.ToLower();
            var contentStrings = content.Split(' ').ToList();
            var mentioned = new List<SocketUser>(message.MentionedUsers);
            var chance = RandomHelper.rand.NextDouble();

            // Respond with an appropriate message dealing with Tarir
            if (contentStrings.Contains("tarir") && chance < 0.25)
            {
                response = await RandomResponse("Tarir Responses");
            }
            // Respond with an appropriate message dealing with Auric Basin
            else if ((content.Contains("auric basin") || contentStrings.Contains("ab")) && chance < 0.25)
            {
                response = await RandomResponse("Auric Basin Responses");
            }
            // Respond with an appropriate message if pinged
            else if (mentioned.Count > 0)
            {
                foreach (var user in mentioned)
                {
                    if (user.Id == selfId)
                    {
                        response = await RandomResponse("Gilded Responses");
                    }
                }
            }
            // Cursed responses
            else if (chance < 0.005)
            {
                response = await RandomResponse("Grab Bag Responses");
            }

            if (response != null)
            {
                await message.Channel.SendMessageAsync(response);
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
    }
}
