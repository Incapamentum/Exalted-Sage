using Discord.WebSocket;
using MongoDB.Driver;
using System;
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

            var selfId = _discordClient.CurrentUser.Id;

            // Bot shouldn't process any messages it sends
            if (message.Author.Id == selfId)
                return;

            var content = message.Content.ToLower();
            var mentioned = new List<SocketUser>(message.MentionedUsers);
            var chance = RandomHelper.rand.NextDouble();

            // Respond with an appropriate message dealing with Tarir
            if (content.Contains("tarir") && chance < 0.25)
            {
                response = await RandomResponse("Tarir Responses");
            }
            // Respond with an appropriate message if pinged
            else if (mentioned.Count > 0 && chance < 0.25)
            {
                foreach (var user in mentioned)
                {
                    if (user.Id == selfId)
                    {
                        response = await RandomResponse("Egg Bearer Responses");
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
