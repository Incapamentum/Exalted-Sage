using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bot.Services;

namespace Bot.Helpers
{
    internal static class ChannelHelper
    {
        /// <summary>
        ///     Retrieves the ID of the named channel.
        /// </summary>
        /// <param name="client">
        ///     The MongoDB client connection to the cluster.
        /// </param>
        /// <param name="docName">
        ///     Name of the doc to look up.
        /// </param>
        /// <param name="channelType">
        ///     Whether the channel is a text or voice channel.
        /// </param>
        /// <param name="channelName">
        ///     The name of the channel to look up their ID.
        /// </param>
        /// <returns>
        ///     The ulong ID of the channel to look for.
        /// </returns>
        internal static async Task<ulong> GetChannelId(MongoClient client,
            string docName, string channelType, string channelName)
        {
            Dictionary<string, ulong> channels = null;
            ulong chanId = 0;

            switch (channelType)
            {
                case "text":
                    channels = await DatabaseService.GetCategoryTextChannels(client, docName);
                    break;
                case "voice":
                    channels = await DatabaseService.GetCategoryVoiceChannels(client, docName);
                    break;
            }

            chanId = channels[channelName];

            return chanId;
        }

        internal static string GetChannelName(ulong id, Dictionary<string, ulong> channels)
        {
            foreach (var pair in channels)
            {
                if (pair.Value == id)
                {
                    return pair.Key;
                }
            }

            return null;
        }

        [Obsolete("Still seeing how this will ultimately fit")]
        internal static string GetChannelNameFromId(ulong id, Dictionary<string, ulong> channels)
        {
            foreach (var pair in channels)
            {
                if (pair.Value == id)
                {
                    return pair.Key;
                }
            }

            return null;
        }
    }
}
