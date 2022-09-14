using Microsoft.Extensions.Configuration;

namespace Bot.Config
{
    internal class AppConfig
    {
        internal DiscordSettings discordSettings;
        internal DatabaseSettings databaseSettings;

        public AppConfig()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile($"Config/settings.json")
                .Build();

            discordSettings = config.GetSection(ReleaseMode.Mode).Get<DiscordSettings>();
            databaseSettings = config.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
        }
    }
}
