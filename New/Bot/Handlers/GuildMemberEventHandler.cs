using Discord;
using Discord.WebSocket;
using NLog;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Bot.Config;
using Bot.Helpers;
using Bot.Services;
using System;

namespace Bot.Handlers
{
    /// <summary>
    ///     Handles any guild member-related events
    /// </summary>
    public class GuildMemberEventHandler : BaseEventHandler
    {

        public GuildMemberEventHandler(DiscordSocketClient discordClient,
            DatabaseService dbService) : base(discordClient, dbService)
        { }

        public async Task
            GuildMemberUpdatedAsync(Cacheable<SocketGuildUser, UInt64> previous,
            SocketGuildUser current)
        {
            var prev = previous.Value;
            var explorerId = await _dbService
                .GrabUserRoleIdByName("Server Management Roles",
                "Explorer");

            // Debugging purposes
            if (ReleaseMode.Mode == "Prod")
            {
                if (CheckIfGainedGilded(prev, current))
                {
                    await current.RemoveRoleAsync(explorerId);
                }
                else if (CheckIfLostGilded(prev, current))
                {
                    await current.AddRoleAsync(explorerId);
                }
            }
            else
            {
                if (CheckIfGainedGilded(prev, current))
                {
                    await current.RemoveRoleAsync(explorerId);
                }
                else
                {
                    await current.AddRoleAsync(explorerId);
                }
            }
        }

        /// <summary>
        ///     Checks to see if the given user contains a
        ///     specified role.
        /// </summary>
        /// <param name="user">
        ///     The user to check for their roles.
        /// </param>
        /// <param name="roleName">
        ///     The name of the role to look for.
        /// </param>
        /// <returns>
        ///     True if the user has the role, false otherwise.
        /// </returns>
        private static bool HasRole(SocketGuildUser user, string roleName)
        {
            var userRoles = user.Roles.ToList();

            if (userRoles.Any(role => role.Name == roleName))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Checks to see if the user has gained or lost the Gilded
        ///     role. This is a role that's assigned/removed by a
        ///     third-party bot.
        /// </summary>
        /// <param name="previous">
        ///     The previous state of the user.
        /// </param>
        /// <param name="current">
        ///     The current state of the user.
        /// </param>
        /// <returns>
        ///     True if the user gained the Gilded role, false
        ///     otherwise.
        /// </returns>
        private static bool CheckIfGainedGilded(SocketGuildUser previous,
            SocketGuildUser current)
        {
            if ((!HasRole(previous, "Gilded") && HasRole(current, "Gilded"))
                && (HasRole(current, "Explorer")))
            {
                return true;
            }

            return false;
        }

        // This has to check whether Gilded has been lost and role doesn't currently
        // have the Explorer
        private static bool CheckIfLostGilded(SocketGuildUser previous,
            SocketGuildUser current)
        {
            if ((HasRole(previous, "Gilded") && !HasRole(current, "Gilded"))
                && (!HasRole(current, "Explorer")))
            {
                return true;
            }

            return false;
        }
    }


}
