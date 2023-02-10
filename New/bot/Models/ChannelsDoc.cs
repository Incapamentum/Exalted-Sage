using System.Collections.Generic;

#nullable enable

namespace Bot.Models
{
    internal class ChannelsDoc : BaseTemplateDoc
    {
        public Dictionary<string, ulong> Channels { get; set; } = null!;
    }
}
