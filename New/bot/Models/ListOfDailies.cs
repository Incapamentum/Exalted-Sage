using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

#nullable enable

namespace bot.Models
{
    /// <summary>
    /// Model representative of the document titled 'List of Daily Achievements'
    /// </summary>
    public class ListOfDailies
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Title")]
        public string DocTitle { get; set; } = null!;

        public string Date { get; set; } = null!;

        public Dictionary<int, string> Achievements { get; set; } = null!;
    }
}
