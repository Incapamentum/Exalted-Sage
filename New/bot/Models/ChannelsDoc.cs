using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

#nullable enable

namespace Bot.Models
{
    internal class ChannelsDoc
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Title")]
        public string DocName { get; set; } = null!;

        public Dictionary<string, ulong> Channels { get; set; } = null!;
    }
}
