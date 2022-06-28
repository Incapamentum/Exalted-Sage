﻿using MongoDB.Driver;
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
        /// 
        /// </summary>
        /// <param name="client">
        ///     The MongoDB client connection to cluster.
        /// </param>
        /// <param name="type">
        ///     The channel type.
        /// </param>
        /// <param name="name">
        ///     The channel name.
        /// </param>
        /// <returns>
        ///     The ID of the channel specified by its type and name.
        /// </returns>
        internal static async Task<ulong> GetChannelId(MongoClient client, string type, string name)
        {
            Dictionary<string, ulong> channels = null;
            ulong chanId = 0;

            switch (type)
            {
                case "broadcast":
                    channels = await DatabaseService.GetBroadcastChannels(client);
                    break;
            }

            if (channels != null)
            {
                chanId = channels[name];
            }

            return chanId;
        }
    }
}