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
    internal class VoiceEventHandler
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly MongoClient _mongoClient;

        public VoiceEventHandler(DiscordSocketClient discordClient, MongoClient mongoClient)
        {
            _discordClient = discordClient;
            _mongoClient = mongoClient;
        }

        public async Task VoiceStateChangeAsync(SocketUser user, SocketVoiceState previous,
            SocketVoiceState current)
        {
            var previousVc = previous.VoiceChannel;

            // Check: a previous state is unavailable
            if (previousVc == null)
                return;

            // Debugging purposes
            if (ReleaseMode.Mode == "ProdSettings")
            {
                var catId = previousVc.Category.Id;

                switch (catId)
                {
                    // LFG
                    case 716054110511825018:
                        await ResetVoiceChannelName(previousVc);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // Need to test something? Put it in here.
                await ResetVoiceChannelName(previousVc);
            }
        }

        private async Task ResetVoiceChannelName(SocketVoiceChannel vc)
        {
            string defaultName;

            var vcId = vc.Id;
            var vcs = await DatabaseService.GetCategoryVoiceChannels(_mongoClient, "lfg-events");

            // Debugging purposes
            if (ReleaseMode.Mode == "ProdSettings")
                defaultName = ChannelHelper.GetChannelNameFromId(vcId, vcs);
            else
                defaultName = "General";

            // Check: only change if name is not default and no users in channel
            if (vc.Name != defaultName && vc.ConnectedUsers.Count == 0)
            {
                await vc.ModifyAsync(x =>
                {
                    x.Name = defaultName;
                });
            }
        }
    }
}
