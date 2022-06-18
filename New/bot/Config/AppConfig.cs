using Microsoft.Extensions.Configuration;

namespace Bot.Config
{
    public class AppConfig
    {
        public AppSettings settings;

        public AppConfig()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile($"Config/settings.json")
                .Build();

            settings = config.GetSection(ReleaseMode.Mode).Get<AppSettings>();
        }
    }
}
