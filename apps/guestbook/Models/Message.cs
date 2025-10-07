using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Guestbook.Models;

public class Message
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("text")]
    [BsonRequired]
    public required string Text { get; set; }

    [BsonElement("createdAt")]
    [BsonRequired]
    public DateTime CreatedAt { get; set; }

    [BsonElement("createdBy")]
    [BsonRequired]
    public required string CreatedBy { get; set; }
}