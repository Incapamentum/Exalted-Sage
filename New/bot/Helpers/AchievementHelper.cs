using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Helpers
{
    public static class AchievementHelper
    {
        /// <summary>
        ///     Collects the names of achievements from their respective IDs.
        /// </summary>
        /// <param name="ids">
        ///     List of achivement IDs.
        /// </param>
        /// <param name="achievements">
        ///     A {achivementID, achivementName} pairing of achievements
        /// </param>
        /// <returns>
        ///     A list of achievement names.
        /// </returns>
        internal static List<string> AchievementNamesFromIds(List<int> ids, Dictionary<int, string> achievements)
        {
            List<string> names = new();

            foreach (int id in ids)
            {
                if (achievements.ContainsKey(id))
                {
                    names.Add(achievements[id]);
                }
            }

            return names;
        }

        /// <summary>
        ///     Collects, if any, names of achievements set to appear tomorrow
        /// </summary>
        /// <param name="achieveNames">
        ///     List of achievement names.
        /// </param>
        /// <param name="achieveWatchlist">
        ///     The watchlist of achievements.
        /// </param>
        /// <returns>
        ///     A list of achievement names set to appear tomorrow.
        /// </returns>
        internal static List<string> AchievementsSetToAppear(List<string> achieveNames, List<string> achieveWatchlist)
        {
            List<string> appearance = new();

            foreach (string name in achieveNames)
            {
                if (achieveWatchlist.Contains(name))
                {
                    appearance.Add(name);
                }
            }

            return appearance;
        }

        /// <summary>
        ///     Get the ID of the named achievement.
        /// </summary>
        /// <param name="name">
        ///     The name of the achievement to find the ID for.
        /// </param>
        /// <param name="achievements">
        ///     The {achievementID, achievementName} mapping.
        /// </param>
        /// <returns>
        ///     The ID of the named achievement if found, 0 otherwise.
        /// </returns>
        internal static int AchievementGetdFromName(string name, Dictionary<int, string> achievements)
        {
            foreach (int id in achievements.Keys)
            {
                if (achievements[id] == name)
                {
                    return id;
                }
            }

            return 0;
        }
    }
}
