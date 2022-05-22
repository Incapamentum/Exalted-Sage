using Bot.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bot.Config;

namespace Bot.Services
{
    /// <summary>
    ///     Exposes a database as a service to the program via public methods
    ///     in the retrieval of specific stored documents
    /// </summary>
    public class DatabaseService
    {
        private readonly MongoDatabaseBase _mDatabase;
        private readonly Dictionary<string, string> _collectionNames;

        /// <summary>
        ///     Establishes a connection with the database specified by the settings,
        ///     and collects the names of each collection in the database.
        /// </summary>
        /// <param name="settings">
        ///     Contains data specific to the database in question
        /// </param>
        public DatabaseService(Dictionary<string, string> settings)
        {
            // Connecting to the database
            var clientSettings = MongoClientSettings.FromConnectionString(settings["ConnectionUri"]);
            clientSettings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var client = new MongoClient(clientSettings);

            _mDatabase = client.GetDatabase(settings["DatabaseName"]) as MongoDatabaseBase;

            // Collecting names of collections that make up the database
            _collectionNames = new Dictionary<string, string>();

            var keys = settings.Keys.ToList<string>();

            foreach (string k in keys)
            {
                if (k.Contains("Collection"))
                {
                    _collectionNames.Add(k, settings[k]);
                }
            }
        }

        /// <summary>
        ///     Retrieves a collection of daily achievements
        /// </summary>
        /// <returns>
        ///     An ID:name pairing
        /// </returns>
        /// 
        public Dictionary<int, string> GetDailyAchievements()
        {
            Dictionary<int, string> dailyAchievements = null;
            MongoCollectionBase<AchievementDoc> achievementCollection;

            achievementCollection = _mDatabase.GetCollection<AchievementDoc>(_collectionNames["AchievementsCollection"])
                as MongoCollectionBase<AchievementDoc>;

            var builder = Builders<AchievementDoc>.Filter;
            var filter = builder.Eq("Title", "Daily Achievements");
            var dailyAchieves = achievementCollection.Find(filter).FirstOrDefault();

            dailyAchievements = dailyAchieves.Achievements;

            return dailyAchievements;
        }

        /// <summary>
        ///     Retrieves a watchlist of daily achievements
        /// </summary>
        /// <returns>
        ///     A list of daily achievements being monitored
        /// </returns>
        public string[] GetDailyWatchlist()
        {
            string[] dailyWatchlist = null;

            MongoCollectionBase<WatchlistDoc> watchlistCollection;

            watchlistCollection = _mDatabase.GetCollection<WatchlistDoc>(_collectionNames["WatchlistsCollection"])
                as MongoCollectionBase<WatchlistDoc>;

            var builder = Builders<WatchlistDoc>.Filter;
            var filter = builder.Eq("Title", "Daily Watchlist");
            var dailyWatch = watchlistCollection.Find(filter).FirstOrDefault();

            dailyWatchlist = dailyWatch.Watchlist;

            return dailyWatchlist;
        }
    }
}
