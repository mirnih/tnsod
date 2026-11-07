using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace RateLimiter.Writer.Infrastructure.Data;

public class RateLimitDbModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("route")]
    public required string Route { get; set; }

    [BsonElement("requests_per_minute")]
    public int RequestsPerMinute { get; set; }
}