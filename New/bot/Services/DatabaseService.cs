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
    public static class DatabaseService
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
        public static MongoClient EstablishConnection(string connectionUri)
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
        public static async Task<Dictionary<int, string>> GetDailyPveAchievements(MongoClient client)
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
        public static async Task<List<string>> GetDailyPveWatchlist(MongoClient client)
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
    }
}
