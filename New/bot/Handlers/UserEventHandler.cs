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
        private readonly DatabaseService _dbService;
        //private readonly MongoClient _mongoClient;

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
            DatabaseService dbService)
        {
            _discordClient = discordClient;
            _dbService = dbService;
            //_mongoClient = mongoClient;
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

            if (!guild.HasAllMembers)
            {
                await guild.DownloadUsersAsync();
            }

            var gUser = user as SocketGuildUser;
            var userRoles = gUser.Roles.ToList();
            var guildRoles = await _dbService
                .GetCategoryServerRoles("Guild Role IDs");

            if (ReleaseMode.Mode == "Prod")
            {
                var broadcastId = await ChannelHelper.
                    GetChannelId(_dbService, "admin-tools",
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

        public async Task UserUpdatedAsync(SocketUser previous,
            SocketUser current)
        {
            // Cast to GuildUser
            var gPrevious = previous as SocketGuildUser;
            var gCurrent = current as SocketGuildUser;

            // Debugging purposes
            if (ReleaseMode.Mode == "Prod")
            {
                // Check to see if user has lost the Gilded role
                if (CheckIfGainedGilded(gPrevious, gCurrent))
                {
                    // Update user's role to the Explorer role
                    var explorerId = await _dbService.
                        GrabUserRoleIdByName("Server Management Roles",
                        "Explorer");

                    await gCurrent.AddRoleAsync(explorerId);
                }
            }
            else
            {
                // Check to see if user has lost the Gilded role
                if (CheckIfGainedGilded(gPrevious, gCurrent))
                {
                    // Update user's role to the Explorer role
                    var explorerId = await _dbService.
                        GrabUserRoleIdByName("Server Management Roles",
                        "Explorer");

                    await gCurrent.AddRoleAsync(explorerId);
                }
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
                if (guildRoles.ContainsValue(role.Id))
                    return true;
            }

            return false;
        }

        // This could probably end up being as a generic, not sure yet
        // Or could just be overloaded
        private async Task NotifyLeadership(SocketGuildUser guildUser,
            SocketTextChannel broadcast)
        {
            var leadershipIds = await _dbService.GetLeadershipIds();

            var embed = new EmbedBuilder
            {
                Description = $"A user with in-game roles has left the server!",
                Color = 0xffc805
            };

            embed.AddField("Discord username", guildUser.Username);

            if (guildUser.Nickname != null)
                embed.AddField("Server nickname", guildUser.Nickname);
            else
                embed.AddField("Server nickname", "unknown");


            embed.WithTitle("User Has Left");

            await broadcast.SendMessageAsync($"Alert <@&{leadershipIds.Item1}>" +
                $" <@&{leadershipIds.Item2}>", embed: embed.Build());
        }

        private static bool CheckForRole(SocketGuildUser gUser, string roleName)
        {
            var gUserRoles = gUser.Roles.ToList();

            if (gUserRoles.Any(role => role.Name == roleName))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Checks to see if the user has gained the Gilded role or
        ///     has lost it.
        /// </summary>
        /// <param name="prevGUser"></param>
        /// <param name="currentGUser"></param>
        /// <returns>
        ///     True if the user gained the Gilded role, false if they lost
        ///     it
        /// </returns>
        private static bool CheckIfGainedGilded(SocketGuildUser prevGUser,
            SocketGuildUser currentGUser)
        {
            if (CheckForRole(prevGUser, "Gilded") &&
                !CheckForRole(currentGUser, "Gilded"))
            {
                return true;
            }

            return false;
        }
    }
}
