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

        [TestMethod]
        public void CheckIfIdRetrievalFails()
        {
            string catName = "Server Management Roles";

            var taskActiveCommander = dbService.
                GrabUserRoleIdByName(catName, "Active Commander");
            taskActiveCommander.Wait();
            var responseActiveCommander = taskActiveCommander.Result;

            var taskAOCommander = dbService.
                GrabUserRoleIdByName(catName, "AO Commander");
            taskAOCommander.Wait();
            var responseAOCommander = taskAOCommander.Result;

            var taskComTag = dbService.
                GrabUserRoleIdByName(catName, "Commander Tag");
            taskComTag.Wait();
            var responseComTag = taskComTag.Result;

            var taskTrader = dbService.
                GrabUserRoleIdByName(catName, "Trader");
            taskTrader.Wait();
            var responseTrader = taskTrader.Result;

            var taskInstanceTrainer = dbService.
                GrabUserRoleIdByName(catName, "Instance Content Trainer");
            taskInstanceTrainer.Wait();
            var responseInstanceTrainer = taskInstanceTrainer.Result;

            var taskExplorer = dbService.
                GrabUserRoleIdByName(catName, "Explorer");
            taskInstanceTrainer.Wait();
            var responseExplorer = taskExplorer.Result;

            Assert.AreEqual((ulong)739588924896444417, responseActiveCommander);
            Assert.AreEqual((ulong)853403399311065109, responseAOCommander);
            Assert.AreEqual((ulong)716046608185163887, responseComTag);
            Assert.AreEqual((ulong)761845222886866995, responseTrader);
            Assert.AreEqual((ulong)1061761196597985370, responseInstanceTrainer);
            Assert.AreEqual((ulong)747362590270947398, responseExplorer);
        }
    }
}