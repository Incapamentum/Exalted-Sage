using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using bot.Config;

namespace bot
{
    class Program
    {
        static void Main(string[] args)
        {
            var appConfig = new AppConfig();

            new ExaltedSage().MainAsync(appConfig.settings.Token).GetAwaiter().GetResult();
        }
    }
}
