using Bot.Config;

namespace Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            var appConfig = new AppConfig();

            new ExaltedSage(appConfig).MainAsync().GetAwaiter().GetResult();
        }
    }
}
