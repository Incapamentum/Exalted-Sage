using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

#nullable enable

using System.Collections.Generic;

namespace Bot.Models
{
    /// <summary>
    /// Model representing the fixed structure of a doc belonging to the
    /// Watchlist collection
    /// </summary>
    public class WatchlistDoc
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Title")]
        public string DocName { get; set; } = null!;

        public string[] Watchlist { get; set; } = null!;
    }
}
