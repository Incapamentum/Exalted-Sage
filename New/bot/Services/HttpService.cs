using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Services
{
    public class HttpService
    {
        public static async Task<JObject> GetTomorrowsDailies()
        {
            HttpClient client = new();

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

            return json;
        }
    }
}
