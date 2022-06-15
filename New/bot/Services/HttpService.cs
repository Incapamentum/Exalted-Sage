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
        ///     IDs for tomorrow
        /// </summary>
        /// <returns></returns>
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

            var pveResult = json["pve"];

            foreach(var item in pveResult.Children())
            {
                tomorrowsIds.Add(item["id"].Value<int>());
            }

            return tomorrowsIds;
        }
    }
}
