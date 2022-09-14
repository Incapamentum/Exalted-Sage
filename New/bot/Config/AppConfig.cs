using Microsoft.Extensions.Configuration;

namespace Bot.Config
{
    internal class AppConfig
    {
        internal Settings settings;
        internal DiscordSettings discordSettings;
        internal DatabaseSettings databaseSettings;

        public AppConfig()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile($"Config/settings.json")
                .Build();

            settings = config.GetSection(ReleaseMode.Mode).Get<Settings>();
        }
    }
}
