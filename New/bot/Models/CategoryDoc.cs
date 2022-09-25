using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

#nullable enable

using System.Collections.Generic;

namespace Bot.Models
{
    internal class CategoryDoc
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Title")]
        public string DocName { get; set; } = null!;

        [BsonElement("CatId")]
        public ulong? CategoryId { get; set; }

        [BsonElement("Text")]
        public Dictionary<string, ulong>? TextChannels { get; set; }

        [BsonElement("Voice")]
        public Dictionary<string, ulong>? VoiceChannels { get; set; }
    }
}
