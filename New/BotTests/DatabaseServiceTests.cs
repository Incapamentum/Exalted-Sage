using Microsoft.VisualStudio.TestTools.UnitTesting;

using MongoDB.Driver;

using Bot.Config;
using Bot.Services;

namespace BotTests
{
    [TestClass]
    public class DatabaseServiceTests
    {
        DatabaseService.SetDatabaseName();
        readonly MongoClient _client = DatabaseService.EstablishConnection(
            "mongodb://localhost:27017");

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}