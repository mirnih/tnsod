using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using RateLimiter.Reader.Domain.Configuration;
using RateLimiter.Reader.Domain.Interfaces;

namespace RateLimiter.Reader.Infrastructure.Data;

public sealed class RateLimitRepository : IRateLimitRepository
{
    private readonly IMongoCollection<RateLimitDbModel> _collection;

    public RateLimitRepository(IOptions<MongoDbOptions> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        var db = client.GetDatabase(options.Value.DatabaseName);
        _collection = db.GetCollection<RateLimitDbModel>(options.Value.CollectionName);
    }

    public async IAsyncEnumerable<RateLimitDbModel> StreamAllAsync(int batchSize = 1000)
    {
        var filter = Builders<RateLimitDbModel>.Filter.Empty;
        var findOptions = new FindOptions<RateLimitDbModel>
        {
            BatchSize = batchSize,
            Sort = Builders<RateLimitDbModel>.Sort.Ascending("_id")
        };

        using var cursor = await _collection.FindAsync(filter, findOptions);
        await foreach (var item in cursor.ToAsyncEnumerable())
            yield return item;
    }

    public async IAsyncEnumerable<ChangeStreamDocument<RateLimitDbModel>> WatchAsync(
        BsonDocument? resumeToken,
        PipelineDefinition<ChangeStreamDocument<RateLimitDbModel>, ChangeStreamDocument<RateLimitDbModel>>? pipeline = null)
    {
        var options = new ChangeStreamOptions
        {
            FullDocument = ChangeStreamFullDocumentOption.UpdateLookup,
            ResumeAfter = resumeToken
        };

        using var cursor = pipeline is null
            ? await _collection.WatchAsync(options)
            : await _collection.WatchAsync(pipeline, options);

        await foreach (var change in cursor.ToAsyncEnumerable())
            yield return change;
    }

    public Task<RateLimitDbModel?> GetByIdAsync(ObjectId id)
    {
        var filter = Builders<RateLimitDbModel>.Filter.Eq("_id", id);
        return _collection.Find(filter).FirstOrDefaultAsync();
    }
}
