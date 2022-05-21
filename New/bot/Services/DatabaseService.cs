using Bot.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Services
{
    /// <summary>
    ///     Exposes the database as a service via public methods for retrieving specific
    ///     stored documents.
    /// </summary>
    public class DatabaseService
    {
        private readonly MongoDatabaseBase _mDatabase;

        public DatabaseService(string connectionUri, string dbName)
        {
            var clientSettings = MongoClientSettings.FromConnectionString(connectionUri);
            clientSettings.ServerApi = new ServerApi(ServerApiVersion.V1);

            var client = new MongoClient(clientSettings);

            _mDatabase = client.GetDatabase(dbName) as MongoDatabaseBase;
        }

        /// <summary>
        ///     Retrieve the achievement ID : achievement name mapping of daily achievements
        /// </summary>
        /// <returns>
        ///     The ID : name mapping of daily achievements
        /// </returns>
        public Dictionary<int, string> GetDailyAchievements()
        {
            Dictionary<int, string> dailyAchievements = null;
            MongoCollectionBase<AchievementDoc> achievementCollection;

            achievementCollection = _mDatabase.GetCollection<AchievementDoc>("achievements")
                as MongoCollectionBase<AchievementDoc>;

            var builder = Builders<AchievementDoc>.Filter;
            var filter = builder.Eq("Title", "Daily Achievements");
            var dailyAchieves = achievementCollection.Find(filter).FirstOrDefault();

            dailyAchievements = dailyAchieves.Achievements;

            return dailyAchievements;
        }

        public string[] GetDailyWatchlist()
        {
            string[] dailyWatchlist = null;
            MongoCollectionBase<WatchlistDoc> watchlistCollection;

            watchlistCollection = _mDatabase.GetCollection<WatchlistDoc>("watchlists")
                as MongoCollectionBase<WatchlistDoc>;

            var builder = Builders<WatchlistDoc>.Filter;
            var filter = builder.Eq("Title", "Daily Watchlist");
            var dailyWatch = watchlistCollection.Find(filter).FirstOrDefault();

            dailyWatchlist = dailyWatch.Watchlist;

            return dailyWatchlist;
        }
    }
}
