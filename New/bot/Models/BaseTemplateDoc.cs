using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

#nullable enable

namespace Bot.Models
{
    /// <summary>
    ///     Base templade model used for standardizing all other docs
    ///     that inherit from it
    /// </summary>
    internal class BaseTemplateDoc
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Title")]
        public string? DocName { get; set; }

        public string? Date { get; set; }
    }
}
