#nullable enable

namespace Bot.Models
{
    /// <summary>
    /// Model representing the fixed structure of a doc belonging to the
    /// Watchlist collection
    /// </summary>
    internal class WatchlistDoc : BaseTemplateDoc
    {
        public string[] Watchlist { get; set; } = null!;
    }
}
