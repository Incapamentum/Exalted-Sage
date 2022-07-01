using Discord.WebSocket;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

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

            var allowableChannels = await DatabaseService.GetGeneralChannels(_mongoClient);

            var selfId = _discordClient.CurrentUser.Id;
            var channelId = message.Channel.Id;

            // Bot shouldn't process any messages it sends
            if (message.Author.Id == selfId)
                return;

            // Bot is only allowed to send messages in the allowable list
            if (!allowableChannels.ContainsValue(channelId))
                return;

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
            else if (chance < 0.05)
            {
                response = await RandomResponse("UwU Responses");
            }

            if (response != null)
            {
                await message.Channel.SendMessageAsync(response);
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
    }
}
