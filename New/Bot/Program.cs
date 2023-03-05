using Bot.Config;

namespace Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            var appConfig = new AppConfig();

            // Testing how to retrieve the specific guild ID
            //Console.WriteLine(appConfig.settings.Guilds["Auric Oasis"]);

            new ExaltedSage(appConfig).MainAsync().GetAwaiter().GetResult();
        }
    }
}
