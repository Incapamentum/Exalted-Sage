using Microsoft.Extensions.Configuration;

namespace Bot.Config
{
    internal class AppConfig
    {
        internal Settings settings;
        //internal DatabaseSettings databaseSettings;

        public AppConfig()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile($"Config/settings.json")
                .Build();

            settings = config.GetSection(ReleaseMode.Mode).Get<Settings>();
            //databaseSettings = config.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
        }
    }
}
