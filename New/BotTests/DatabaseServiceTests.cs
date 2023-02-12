using Microsoft.VisualStudio.TestTools.UnitTesting;

using Bot.Config;
using Bot.Services;

namespace BotTests
{
    [TestClass]
    public class DatabaseServiceTests
    {
        readonly DatabaseService dbService = new("Bot_Test",
            "mongodb://localhost:27017");

        [TestMethod]
        public void CheckIfDocRetrievalFails()
        {
            var taskAchieves = dbService.GetDailyPveAchievements();
            taskAchieves.Wait();
            var responseAchieves = taskAchieves.Result;

            var taskCategories = dbService
                .GetCategoryTextChannels("admin-tools");
            taskCategories.Wait();
            var responseCategories = taskCategories.Result;

            var taskResponses = dbService.GetResponses("Grab Bag Responses");
            taskResponses.Wait();
            var responseResponses = taskResponses.Result;

            var taskRoles = dbService.GetLeadershipIds();
            taskRoles.Wait();
            var responseRoles = taskRoles.Result;

            var taskWatchlist = dbService.GetDailyPveWatchlist();
            taskWatchlist.Wait();
            var responseWatchlist = taskWatchlist.Result;

            Assert.IsNotNull(responseAchieves);
            Assert.IsNotNull(responseCategories);
            Assert.IsNotNull(responseResponses);    
            Assert.IsNotNull(responseRoles);
            Assert.IsNotNull(responseWatchlist);
        }
    }
}