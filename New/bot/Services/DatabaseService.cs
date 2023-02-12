using Bot.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Bot.Config;

namespace Bot.Services
{
    /// <summary>
    ///     Exposes a database as a service to the program via public methods
    ///     in the retrieval of stored documents.
    /// </summary>
    public class DatabaseService
    {
        private readonly string _dbName;
        private readonly MongoClient _client;

        /// <summary>
        ///     Establishes a connection to a NoSQL database.
        /// </summary>
        /// <param name="databaseName">
        ///     
        /// </param>
        /// <param name="connectionUri">
        ///     The URI string necessary to establish a connection to the
        ///     database.
        /// </param>
        public DatabaseService(string databaseName, string connectionUri)
        {
            _dbName = databaseName;

            var clientSettings = MongoClientSettings.
                FromConnectionString(connectionUri);
            clientSettings.ServerApi = new ServerApi(ServerApiVersion.V1);

            _client = new MongoClient(clientSettings);
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="name"></param>
        //public static void SetDatabaseName(string name)
        //{
        //    dbName = name;
        //}

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
        //public static MongoClient EstablishConnection(string connectionUri)
        //{
        //    MongoClient client = null;

        //    var clientSettings = MongoClientSettings.FromConnectionString(connectionUri);
        //    clientSettings.ServerApi = new ServerApi(ServerApiVersion.V1);
        //    client = new MongoClient(clientSettings);

        //    return client;
        //}

        [Obsolete("Better off to be supported in a separate, helper class vs " +
            "in this class.")]
        public static async Task<ulong> GetAdminAlertChannel(MongoClient client)
        {
            var channels = await GetCategoryTextChannels(client, "admin-tools");

            return channels["bot-alerts"];
        }

        /// <summary>
        ///     Accesses the 'achievements' collection in the database to
        ///     retrieve a (ID, name) mapping of all (currently) possible
        ///     daily PvE achievements.
        /// </summary>
        /// <param name="client">
        ///     The MongoDB client connection to cluster.
        /// </param>
        /// <returns>
        ///     A {achievementID, achivementName} pairing of daily PVE
        ///     achievements.
        /// </returns>
        public async Task<Dictionary<int, string>> GetDailyPveAchievements()
        {
            var achievementCollection = GrabCollection<AchievementDoc>("achievements");
            var dailyAchieves = await GrabDocument(achievementCollection,
                "Daily Achievements");

            return dailyAchieves.Achievements;
        }

        /// <summary>
        ///     Accesses the 'watchlist' collection in the database to retrieve
        ///     a list of a subset of daily PvE achievements. This subset
        ///     corresponds to dailies meant to watch out for.
        /// </summary>
        /// <param name="client">
        ///     The MongoDB client connection to cluster.
        /// </param>
        /// <returns>
        ///     A list of strings of the daily PVE achievement watchlist
        /// </returns>
        public async Task<List<string>> GetDailyPveWatchlist()
        {
            var watchlistCollection = GrabCollection<WatchlistDoc>("watchlists");
            var watchlistDoc = await GrabDocument(watchlistCollection,
                "Daily Watchlist");

            return watchlistDoc.Watchlist.ToList();
        }

        /// <summary>
        ///     Accesses the 'responses' collection in the database to retrieve
        ///     a list of responses. Each doc in this collection corresponds with
        ///     a particular type.
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
        public async Task<List<string>> GetResponses(string responseType)
        {
            var responseCollection = GrabCollection<ResponseDoc>("responses");
            var responseDoc = await GrabDocument(responseCollection, responseType);

            return responseDoc.Responses.ToList();
        }

        /// <summary>
        ///     Accesses the 'categories' collection in the database to
        ///     retrieve the (name, ID) mapping of the text channels found
        ///     within the category.
        /// </summary>
        /// <param name="client">
        ///     The MongoDB client connection to the cluster.
        /// </param>
        /// <param name="catName">
        ///     The name of the category doc to access.
        /// </param>
        /// <returns>
        ///     A (string, ulong) mapping of the text channels found in the
        ///     specified category.
        /// </returns>
        public async Task<Dictionary<string, ulong>>
            GetCategoryTextChannels(string catName)
        {
            var categoryCollection = GrabCollection<CategoryDoc>("categories");
            var cat = await GrabDocument(categoryCollection, catName);

            return cat.TextChannels;
        }

        /// <summary>
        ///     Accesses the 'categories' collection in the database to retrieve
        ///     the (name, ID) mapping of the voice channels found within the
        ///     category.
        /// </summary>
        /// <param name="client">
        ///     The MongoDB client connection to the cluster.
        /// </param>
        /// <param name="catName">
        ///     The name of the category doc to access.
        /// </param>
        /// <returns>
        ///     A (string, ulong) mapping of the voice channels found in the specified
        ///     category.
        /// </returns>
        public async Task<Dictionary<string, ulong>>
            GetCategoryVoiceChannels(string catName)
        {
            var categoryCollection = GrabCollection<CategoryDoc>("categories");
            var cat = await GrabDocument(categoryCollection, catName);

            return cat.VoiceChannels;
        }

        public async Task<Dictionary<string, ulong>>
            GetCategoryServerRoles(string catName)
        {
            var categoryCollection = GrabCollection<RolesDoc>("roles");
            var cat = await GrabDocument(categoryCollection, catName);

            return cat.Roles;
        }

        public async Task<Tuple<ulong, ulong>>
            GetLeadershipIds(MongoClient client)
        {
            var categoryCollection = GrabCollection<RolesDoc>("roles");
            var cat = await GrabDocument(categoryCollection, "Guild Role IDs");

            return Tuple.Create(cat.Roles["Exalted"], cat.Roles["Ascended"]);
        }

        /// <summary>
        ///     Retrieves the collection of channels to broadcast messages to.
        /// </summary>
        /// <param name="client">
        ///     The MongoDB client connection to cluster.
        /// </param>
        /// <returns>
        ///     A {string, ulong} mapping of channnels to broadcast to.
        /// </returns>
        /// 
        [Obsolete("Collection 'channels' will soon be deprecated." +
            "Will be removed in a future build.")]
        internal static async Task<Dictionary<string, ulong>> GetBroadcastChannels(MongoClient client)
        {
            var channelsCollection = GrabCollection<ChannelsDoc>(client, "channels");
            var broadcastDoc = await GrabDocument(channelsCollection, "Broadcast Channels");

            return broadcastDoc.Channels;
        }

        /// <summary>
        ///     Retrieves the collection of general channels.
        /// </summary>
        /// <param name="client">
        ///     The MongoDB client connection to cluster.
        /// </param>
        /// <returns>
        ///     A [string, ulong] mapping of general channels.
        /// </returns>
        /// 
        [Obsolete("Collection 'channels' will soon be deprecated." +
            "Will be removed in a future build.")]
        internal static async Task<Dictionary<string, ulong>> GetGeneralChannels(MongoClient client)
        {
            var channelsCollection = GrabCollection<ChannelsDoc>(client, "channels");
            var generalDoc = await GrabDocument(channelsCollection, "General Channels");

            return generalDoc.Channels;
        }

        /// <summary>
        ///     Retrieves the collection of event voice channels.
        /// </summary>
        /// <param name="client">
        ///     The MongoDB client connecton to cluster.
        /// </param>
        /// <returns>
        ///     A [string, ulong] mapping of event voice channels.
        /// </returns>
        [Obsolete("Collection 'channels' will soon be deprecated." +
            "Will be removed in a future build.")]
        internal static async Task<Dictionary<string, ulong>> GetEventVoiceChannels(MongoClient client)
        {
            var channelsCollection = GrabCollection<ChannelsDoc>(client, "channels");
            var vcDoc = await GrabDocument(channelsCollection, "Event Voice Channels");

            return vcDoc.Channels;
        }

        /// <summary>
        ///     Retrieves the collection of supervised channels.
        /// </summary>
        /// <param name="client">
        ///     The MongoDB client connection to cluster.
        /// </param>
        /// <returns>
        ///     A [string, ulong] mapping of supervised channels.
        /// </returns>
        /// 
        [Obsolete("Collection 'channels' will soon be deprecated." +
            "Will be removed in a future build.")]
        internal static async Task<Dictionary<string, ulong>> GetSupervisedChannels(MongoClient client)
        {
            var channelsCollection = GrabCollection<ChannelsDoc>(client, "channels");
            var supervisedDoc = await GrabDocument(channelsCollection, "Supervised Channels");

            return supervisedDoc.Channels;
        }

        /// <summary>
        ///     Generic helper function in the retrieval of collections
        ///     from the same database.
        /// </summary>
        /// <typeparam name="TModel">
        ///     The model type.
        /// </typeparam>
        /// <param name="client">
        ///     The client connection to the Mongo database.
        /// </param>
        /// <param name="type">
        ///     The name of the collection to retrieve.
        /// </param>
        /// <returns></returns>
        private MongoCollectionBase<TModel> GrabCollection<TModel>(string type)
        {
            MongoCollectionBase<TModel> collection;

            var database = _client.GetDatabase(_dbName) as MongoDatabaseBase;

            collection = database.GetCollection<TModel>(type) as MongoCollectionBase<TModel>;

            return collection;
        }

        /// <summary>
        ///     Generic helper function in the retrieval of a
        ///     specified document within the collection.
        /// </summary>
        /// <typeparam name="TModel">
        ///     The type of model/doc.
        /// </typeparam>
        /// <param name="collection">
        ///     The Mongo collection to look for the doc in.
        /// </param>
        /// <param name="title">
        ///     The title of the doc to look for.
        /// </param>
        /// <returns></returns>
        private static async Task<TDoc> GrabDocument<TDoc>(MongoCollectionBase<TDoc> collection, string title)
        {
            TDoc doc;

            var builder = Builders<TDoc>.Filter;
            var filter = builder.Eq("Title", title);
            doc = await collection.Find(filter).FirstOrDefaultAsync();

            return doc;
        }
    }
}
