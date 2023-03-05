using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using System.Collections.Generic;

#nullable enable

namespace Bot.Models
{
    internal class CategoryDoc : BaseTemplateDoc
    {
        [BsonElement("CatId")]
        public ulong? CategoryId { get; set; }

        [BsonElement("Text")]
        public Dictionary<string, ulong>? TextChannels { get; set; }

        [BsonElement("Voice")]
        public Dictionary<string, ulong>? VoiceChannels { get; set; }
    }
}
