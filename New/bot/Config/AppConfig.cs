﻿using Microsoft.Extensions.Configuration;

namespace bot.Config
{
    public class AppConfig
    {
        public AppSettings settings;

        public AppConfig()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile($"Config/settings.json")
                .Build();

            settings = config.GetSection(nameof(AppSettings)).Get<AppSettings>();        
        }
    }
}
