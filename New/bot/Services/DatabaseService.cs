using Bot.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Services
{
    /// <summary>
    ///     Exposes a database as a service to the program via public methods
    ///     in the retrieval of stored documents.
    /// </summary>
    internal static class DatabaseService
    {
        /// <summary>
        ///     Establishes a connection to the MongoDB cluster
        ///     specified by the URI string.
        /// </summary>
        /// <param name="connectionUri">
        ///     The URI string to connect to the MongoDB service.
        /// </param>
        /// <returns>
        ///     A client interface connection to the MongoDB cluster.
        /// </returns>
        internal static MongoClient EstablishConnection(string connectionUri)
        {
            MongoClient client = null;

            var clientSettings = MongoClientSettings.FromConnectionString(connectionUri);
            clientSettings.ServerApi = new ServerApi(ServerApiVersion.V1);
            client = new MongoClient(clientSettings);

            return client;
        }

        /// <summary>
        ///     Retrieves a collection of daily achievements. No guarantees of something being returned.
        /// </summary>
        /// <param name="client">
        ///     The MongoDB client connection to cluster.
        /// </param>
        /// <returns>
        ///     A {achievementID, achivementName} pairing of daily PVE achievements.
        /// </returns>
        internal static async Task<Dictionary<int, string>> GetDailyPveAchievements(MongoClient client)
        {
            Dictionary<int, string> dailyAchievements = null;
            MongoCollectionBase<AchievementDoc> achievementCollection;

            var database = client.GetDatabase("Auric_Oasis") as MongoDatabaseBase;

            achievementCollection = database.GetCollection<AchievementDoc>("achievements")
                as MongoCollectionBase<AchievementDoc>;

            var builder = Builders<AchievementDoc>.Filter;
            var filter = builder.Eq("Title", "Daily Achievements");
            var dailyAchieves = await achievementCollection.Find(filter).FirstOrDefaultAsync();

            dailyAchievements = dailyAchieves.Achievements;

            return dailyAchievements;
        }

        /// <summary>
        ///     Retrieves the watchlist of daily PVE achievements. No guarantees of something being returned.
        /// </summary>
        /// <param name="client">
        ///     The MongoDB client connection to cluster.
        /// </param>
        /// <returns>
        ///     A list of strings of the daily PVE achievement watchlist
        /// </returns>
        internal static async Task<List<string>> GetDailyPveWatchlist(MongoClient client)
        {
            string[] dailyWatchlist = null;
            MongoCollectionBase<WatchlistDoc> watchlistCollection;

            var database = client.GetDatabase("Auric_Oasis") as MongoDatabaseBase;

            watchlistCollection = database.GetCollection<WatchlistDoc>("watchlists")
                as MongoCollectionBase<WatchlistDoc>;

            var builder = Builders<WatchlistDoc>.Filter;
            var filter = builder.Eq("Title", "Daily Watchlist");
            var dailyWatch = await watchlistCollection.Find(filter).FirstOrDefaultAsync();

            dailyWatchlist = dailyWatch.Watchlist;

            return dailyWatchlist.ToList();
        }

        /// <summary>
        ///     Retrieves a doc of type Response.
        /// </summary>
        /// <param name="client">
        ///     The MongoDB client connection to cluster.
        /// </param>
        /// <param name="responseType">
        ///     The title of the Response doc to look for.
        /// </param>
        /// <returns>
        ///     A list of strings representing response callbacks the bot makes.
        /// </returns>
        internal static async Task<List<string>> GetResponses(MongoClient client, string responseType)
        {
            string[] responses = null;
            MongoCollectionBase<ResponseDoc> responseCollection;

            var database = client.GetDatabase("Auric_Oasis") as MongoDatabaseBase;

            responseCollection = database.GetCollection<ResponseDoc>("responses")
                as MongoCollectionBase<ResponseDoc>;

            var builder = Builders<ResponseDoc>.Filter;
            var filter = builder.Eq("Title", responseType);
            var responseList = await responseCollection.Find(filter).FirstOrDefaultAsync();

            responses = responseList.Responses;

            return responses.ToList();
        }

        /// <summary>
        ///     Retrieves a collection of channels to broadcast messages to.
        /// </summary>
        /// <param name="client">
        ///     The MongoDB client connection to cluster.
        /// </param>
        /// <returns>
        ///     A {string, ulong} mapping of channnels to broadcast to.
        /// </returns>
        internal static async Task<Dictionary<string, ulong>> GetBroadcastChannels(MongoClient client)
        {
            Dictionary<string, ulong> broadcastChannels = null;
            MongoCollectionBase<ChannelsDoc> channelsCollection;

            var database = client.GetDatabase("Auric_Oasis") as MongoDatabaseBase;

            channelsCollection = database.GetCollection<ChannelsDoc>("channels")
                as MongoCollectionBase<ChannelsDoc>;

            var builder = Builders<ChannelsDoc>.Filter;
            var filter = builder.Eq("Title", "Broadcast Channels");
            var broadcastList = await channelsCollection.Find(filter).FirstOrDefaultAsync();

            broadcastChannels = broadcastList.Channels;

            return broadcastChannels;
        }
    }
}
