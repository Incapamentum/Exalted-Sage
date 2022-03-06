using Discord;
using Discord.WebSocket;
using System.IO;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bot.Services
{
    /// <summary>
    ///     Seems like the "standard" way of having this filter is through a database. Might
    ///     have some work cut out for me I guess. Pain
    ///     
    ///     Might be useful (or necessary) to include the MongoDB API here, since it seems
    ///     it'll have to do some access to it
    ///     
    ///     In hindsight, will this ultimately be needed?
    /// </summary>
    public class FilterService
    {
        private readonly Filter _filter;
    }

    public class Filter
    {
        //[JsonProperty]
    }
}
