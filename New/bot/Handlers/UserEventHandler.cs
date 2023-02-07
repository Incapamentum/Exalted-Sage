using Discord;
using Discord.WebSocket;
using NLog;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bot.Config;
using Bot.Helpers;
using Bot.Services;

namespace Bot.Handlers
{
    /// <summary>
    ///     Handles any user-related events
    /// </summary>
    public class UserEventHandler
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly MongoClient _mongoClient;

        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Handles specific event-driven processes triggered by
        ///     users.
        /// </summary>
        /// <param name="discordClient">
        ///     The SocketClient interface to interact with Discord.
        /// </param>
        /// <param name="mongoClient">
        /// </param>
        public UserEventHandler(DiscordSocketClient discordClient,
            MongoClient mongoClient)
        {
            _discordClient = discordClient;
            _mongoClient = mongoClient;
        }

        /// <summary>
        ///     Fired off whenever a user leaves the server.
        /// </summary>
        /// <param name="guild">
        ///     The server the user belonged to.
        /// </param>
        /// <param name="user">
        ///     The user that left the server.
        /// </param>
        /// <returns>
        ///     None.
        /// </returns>
        /// <remarks>
        ///     Would be cool to have a "global" notify leadership function
        /// </remarks>
        public async Task UserLeftAsync(SocketGuild guild, SocketUser user)
        {
            SocketTextChannel broadcastChannel;

            var gUser = user as SocketGuildUser;
            var userRoles = gUser.Roles.ToList();
            var guildRoles = await DatabaseService
                .GetCategoryServerRoles(_mongoClient, "Guild Role IDs");

            if (ReleaseMode.Mode == "Prod")
            {
                var broadcastId = await ChannelHelper.
                    GetChannelId(_mongoClient, "admin-tools",
                    "text", "bot-alerts");
                broadcastChannel = _discordClient.GetChannel(broadcastId)
                    as SocketTextChannel;
            }
            // Debugging purposes
            else
            {
                Logger.Info("Debug mode!");

                broadcastChannel = _discordClient
                    .GetChannel(1013185367924547654) as SocketTextChannel;
            }

            if (HasInGameRoles(userRoles, guildRoles))
            {
                await NotifyLeadership(gUser, broadcastChannel);
            }
        }

        /// <summary>
        ///     Checks to see if the user has any in-game guild roles.
        /// </summary>
        /// <param name="userRoles">
        ///     The roles of the user.
        /// </param>
        /// <param name="guildRoles">
        ///     The in-game guild roles.
        /// </param>
        /// <returns></returns>
        private static bool HasInGameRoles(List<SocketRole> userRoles,
            Dictionary<string, ulong> guildRoles)
        {
            foreach (var role in userRoles)
            {
                if (guildRoles.ContainsKey(role.Name))
                    return true;
            }

            return false;
        }

        // This could probably end up being as a generic, not sure yet
        // Or could just be overloaded
        private async Task NotifyLeadership(SocketGuildUser guildUser,
            SocketTextChannel broadcast)
        {
            var leadershipIds = await DatabaseService.GetLeadershipIds(_mongoClient);

            var embed = new EmbedBuilder
            {
                Description = $"A user with in-game roles has left the server!",
                Color = 0xffc805
            };

            embed.AddField("Discord username", guildUser.Username);
            embed.AddField("Server nickname", guildUser.Nickname);

            embed.WithTitle("User Has Left");

            await broadcast.SendMessageAsync($"Alert <@{leadershipIds.Item1}>" +
                $" <@{leadershipIds.Item2}>", embed: embed.Build());
        }
    }
}
