using Microsoft.Extensions.Configuration;

namespace Bot.Config
{
    public class AppConfig
    {
        public DiscordSettings discordSettings;
        public DatabaseSettings databaseSettings;

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
