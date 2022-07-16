using Discord.WebSocket;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bot.Config;
using Bot.Helpers;
using Bot.Services;

namespace Bot.Handlers
{
    /// <summary>
    ///     Handler class that handles state of voice channels
    /// </summary>
    internal class VoiceHandler
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly MongoClient _mongoClient;

        public VoiceHandler(DiscordSocketClient discordClient, MongoClient mongoClient)
        {
            _discordClient = discordClient;
            _mongoClient = mongoClient;
        }

        public async Task VoiceStateChangeAsync(SocketUser user, SocketVoiceState previous,
            SocketVoiceState current)
        {
            string defaultVcName;
            ulong channelId;

            var previousVc = previous.VoiceChannel;
            var eventVcs = await DatabaseService.GetEventVoiceChannels(_mongoClient);
            var eventIds = eventVcs.Values.ToList();

            // Check: a previous state is unavailable
            if (previousVc == null)
                return;

            // Check: only process the Event Channels
            if (!eventIds.Contains(previousVc.Id))
                return;

            // Debugging purposes
            if (ReleaseMode.Mode == "DevSettings")
            {
                channelId = 720690834638372950;
                defaultVcName = "General";
            }
            // Obtaining the default VC name through channel ID
            else
            {
                channelId = previousVc.Id;
                defaultVcName = ChannelHelper.GetChannelNameFromId(channelId, eventVcs);
            }

            // Check: only change name if it's not default and if no users in channel
            if (previousVc.Name != defaultVcName && previousVc.ConnectedUsers.Count == 0)
            {
                await previousVc.ModifyAsync(x =>
                {
                    x.Name = defaultVcName;
                });
            }
        }
    }
}
