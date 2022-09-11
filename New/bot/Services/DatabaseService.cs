using Bot.Models;
using MongoDB.Driver;
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
    internal static class DatabaseService
    {
        static string dbName;

        /// <summary>
        ///     
        /// </summary>
        /// <param name="name"></param>
        internal static void SetDatabaseName(string name)
        {
            dbName = name;
        }

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
            var achievementCollection = GrabCollection<AchievementDoc>(client, "achievements");
            var dailyAchieves = await GrabDocument(achievementCollection, "Daily Achievements");

            return dailyAchieves.Achievements;
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
            var watchlistCollection = GrabCollection<WatchlistDoc>(client, "watchlists");
            var watchlistDoc = await GrabDocument(watchlistCollection, "Daily Watchlist");

            return watchlistDoc.Watchlist.ToList();
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
            var responseCollection = GrabCollection<ResponseDoc>(client, "responses");
            var responseDoc = await GrabDocument(responseCollection, responseType);

            return responseDoc.Responses.ToList();
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
        private static MongoCollectionBase<TModel> GrabCollection<TModel>(MongoClient client, string type)
        {
            MongoCollectionBase<TModel> collection;

            var database = client.GetDatabase(dbName) as MongoDatabaseBase;

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
        private static async Task<TModel> GrabDocument<TModel>(MongoCollectionBase<TModel> collection, string title)
        {
            TModel doc;

            var builder = Builders<TModel>.Filter;
            var filter = builder.Eq("Title", title);
            doc = await collection.Find(filter).FirstOrDefaultAsync();

            return doc;
        }
    }
}
