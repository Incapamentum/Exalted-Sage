using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

#nullable enable

namespace bot.Models
{
    /// <summary>
    /// Model representative of the document titled 'Daily Watchlist'
    /// </summary>
    public class DailyWatchList
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Title")]
        public string DocTitle { get; set; } = null!;

        public string[] Watchlist { get; set; } = null!;
    }
}
