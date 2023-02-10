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
        public string Id { get; set; } = null!;

        [BsonElement("Title")]
        public string DocName { get; set; } = null!;

        public string Date { get; set; } = null!;
    }
}
