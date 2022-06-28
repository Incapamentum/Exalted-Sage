﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

#nullable enable

namespace Bot.Models
{
    /// <summary>
    ///     Model representing the fixed structure of a BSON doc containing
    ///     specific responses as a list.
    /// </summary>
    internal class ResponseDoc
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Title")]
        public string DocName { get; set; } = null!;

        public string Date { get; set; } = null!;

        public string[] Responses { get; set; } = null!;
    }
}