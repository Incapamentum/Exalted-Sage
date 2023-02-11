using System;
using System.Collections.Generic;

#nullable enable

namespace Bot.Models
{
    [Obsolete("The 'channels' category is planned to be removed in a future " +
        "update, so this should not be used until it's removed")]
    internal class ChannelsDoc : BaseTemplateDoc
    {
        public Dictionary<string, ulong>? Channels { get; set; }
    }
}
