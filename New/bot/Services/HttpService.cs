using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bot.Services
{
    /// <summary>
    ///     Exposes specific methods that perform HTTP requests in retrieving
    ///     data.
    /// </summary>
    public static class HttpService
    {
        /// <summary>
        ///     Performs an HTTP request in retrieving the daily achievement 
        ///     IDs for tomorrow. Guaranteed to return something.
        /// </summary>
        /// <returns>
        ///     List of achievement IDs
        /// </returns>
        public static async Task<List<int>> GetTomorrowsPveDailiesId()
        {
            HttpClient client = new();
            List<int> tomorrowsIds = new();

            HttpResponseMessage response;
            response = await client.GetAsync("https://api.guildwars2.com/v2/achievements/daily/tomorrow");

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message: {0}", e.Message);

                return null;
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseBody);

            var pveResult = json.SelectToken("pve");

            foreach(var item in pveResult.Children())
            {
                tomorrowsIds.Add(item["id"].Value<int>());
            }

            return tomorrowsIds;
        }

        /// <summary>
        ///     Performs an HTTP request in retrieving the description of an
        ///     achievement by ID.
        /// </summary>
        /// <param name="id">
        ///     The ID of the achievement.
        /// </param>
        /// <returns>
        ///     Description of the achievement.
        /// </returns>
        public static async Task<string> ObtainAchievementDescription(int id)
        {
            HttpClient client = new();

            HttpResponseMessage response;
            response = await client.GetAsync("https://api.guildwars2.com/v2/achievements/" + id.ToString());

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message: {0}", e.Message);

                return null;
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(responseBody);

            var description = json.SelectToken("description");

            return description.Value<string>();
        }
    }
}
