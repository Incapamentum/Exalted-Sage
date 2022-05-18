﻿using System.Collections.Generic;

namespace bot.Config
{
    public class AppSettings
    {
        public string Token { get; set; }
        public Dictionary<string, ulong> Guilds { get; set; }
        public Dictionary<string, string> AuricOasisDatabase { get; set; }
    }
}
