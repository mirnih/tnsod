using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using RateLimiter.Reader.Domain.Configuration;
using RateLimiter.Reader.Domain.Entities;
using RateLimiter.Reader.Domain.Interfaces;
using MongoChangeStreamOperationType = MongoDB.Driver.ChangeStreamOperationType;

namespace RateLimiter.Reader.Infrastructure.Data;

public sealed class RateLimitRepository : IRateLimitRepository
{
    private readonly IMongoCollection<RateLimitDbModel> _collection;
    private readonly IMapperFactory _mapperFactory;
    private BsonDocument? _resumeToken;

    public RateLimitRepository(IOptions<MongoDbOptions> options, IMapperFactory mapperFactory)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        var db = client.GetDatabase(options.Value.DatabaseName);
        _collection = db.GetCollection<RateLimitDbModel>(options.Value.CollectionName);
        _mapperFactory = mapperFactory;
    }

    public async IAsyncEnumerable<RateLimit> StreamAllAsync(int batchSize = 1000)
    {
        var filter = Builders<RateLimitDbModel>.Filter.Empty;
        var findOptions = new FindOptions<RateLimitDbModel>
        {
            BatchSize = batchSize,
            Sort = Builders<RateLimitDbModel>.Sort.Ascending("_id")
        };

        using var cursor = await _collection.FindAsync(filter, findOptions);
        await foreach (var dbModel in cursor.ToAsyncEnumerable())
        {
            yield return _mapperFactory.RateLimit.ToDomain(dbModel);
        }
    }

    public async IAsyncEnumerable<RateLimitChange> WatchAsync()
    {
        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<RateLimitDbModel>>()
            .Match(x =>
                x.OperationType == MongoChangeStreamOperationType.Insert
                || x.OperationType == MongoChangeStreamOperationType.Replace
                || x.OperationType == MongoChangeStreamOperationType.Update
                || x.OperationType == MongoChangeStreamOperationType.Delete);

        var options = new ChangeStreamOptions
        {
            FullDocument = ChangeStreamFullDocumentOption.UpdateLookup,
            ResumeAfter = _resumeToken
        };

        using var cursor = await _collection.WatchAsync(pipeline, options);

        await foreach (var change in cursor.ToAsyncEnumerable())
        {
            _resumeToken = change.ResumeToken;

            var operationType = change.OperationType switch
            {
                MongoChangeStreamOperationType.Insert => ChangeOperationType.Insert,
                MongoChangeStreamOperationType.Update => ChangeOperationType.Update,
                MongoChangeStreamOperationType.Replace => ChangeOperationType.Replace,
                MongoChangeStreamOperationType.Delete => ChangeOperationType.Delete,
                _ => throw new InvalidOperationException($"Unsupported operation type: {change.OperationType}")
            };

            if (operationType == ChangeOperationType.Delete)
            {
                var deletedId = change.DocumentKey["_id"].AsObjectId.ToString();
                yield return new RateLimitChange(operationType, null, deletedId);
            }
            else
            {
                var fullDocument = change.FullDocument;
                if (fullDocument is null && change.DocumentKey is not null)
                {
                    fullDocument = await GetByIdInternalAsync(change.DocumentKey["_id"].AsObjectId);
                }

                if (fullDocument is not null)
                {
                    var domainModel = _mapperFactory.RateLimit.ToDomain(fullDocument);
                    yield return new RateLimitChange(operationType, domainModel, fullDocument.Id);
                }
            }
        }
    }

    private Task<RateLimitDbModel?> GetByIdInternalAsync(ObjectId id)
    {
        var filter = Builders<RateLimitDbModel>.Filter.Eq("_id", id);
        return _collection.Find(filter).FirstOrDefaultAsync();
    }
}
