using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

#nullable enable

namespace Bot.Models
{
    internal class BaseTemplateDoc
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("Title")]
        public string DocName { get; set; } = null!;

        public string Date { get; set; } = null!;
    }
}
