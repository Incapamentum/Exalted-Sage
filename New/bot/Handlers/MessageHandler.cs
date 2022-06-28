using Discord.WebSocket;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var selfId = _discordClient.CurrentUser.Id;

            // Bot shouldn't process any messages it sends
            if (message.Author.Id == selfId)
                return;

            var content = message.Content.ToLower();
            var rand = new Random(16);
            var mentioned = new List<SocketUser>(message.MentionedUsers);

            // Some of these execution paths could be made compact into the form
            // of a function, i.e.
            //
            //      var response
            //      var index
            //      await message
            //
            // are all the same implementation  

            // Respond with an appropriate message dealing with Tarir
            if (content.Contains("tarir") && rand.NextDouble() < 0.25)
            {
                var response = await DatabaseService.GetResponses(_mongoClient, "Tarir Responses");
                var index = rand.Next(response.Count);

                await message.Channel.SendMessageAsync(response[index]);
            }

            // Respond with an appropriate message if pinged
            if (mentioned.Count > 0)
            {
                foreach (var user in mentioned)
                {
                    if (user.Id == selfId)
                    {
                        var response = await DatabaseService.GetResponses(_mongoClient, "Egg Bearer Responses");
                        var index = rand.Next(response.Count);

                        await message.Channel.SendMessageAsync(response[index]);
                    }
                }
            }
        }
    }
}
