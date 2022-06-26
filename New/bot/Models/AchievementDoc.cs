using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

#nullable enable

using System.Collections.Generic;

namespace Bot.Models
{
    /// <summary>
    /// Model representing the fixed structure of a doc belonging to the
    /// Achievements collection
    /// </summary>
    internal class AchievementDoc
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Title")]
        public string DocName { get; set; } = null!;

        public string Date { get; set; } = null!;

        public Dictionary<int, string> Achievements { get; set; } = null!;
    }
}
