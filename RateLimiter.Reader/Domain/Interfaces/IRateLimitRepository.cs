using MongoDB.Bson;
using MongoDB.Driver;
using RateLimiter.Reader.Infrastructure.Data;
namespace RateLimiter.Reader.Domain.Interfaces;

public interface IRateLimitRepository
{
    IAsyncEnumerable<RateLimitDbModel> StreamAllAsync(int batchSize = 1000);
    IAsyncEnumerable<ChangeStreamDocument<RateLimitDbModel>> WatchAsync(
        BsonDocument? resumeToken,
        PipelineDefinition<ChangeStreamDocument<RateLimitDbModel>, ChangeStreamDocument<RateLimitDbModel>>? pipeline = null);
    Task<RateLimitDbModel?> GetByIdAsync(ObjectId id);
}