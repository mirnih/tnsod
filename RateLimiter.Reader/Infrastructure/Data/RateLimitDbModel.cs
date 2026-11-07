using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RateLimiter.Reader.Infrastructure.Data;

public class RateLimitDbModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("route")]
    public string Route { get; set; } = string.Empty;

    [BsonElement("requests_per_minute")]
    public int RequestsPerMinute { get; set; }
}