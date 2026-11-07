using System.Collections.Concurrent;
using MongoDB.Bson;
using MongoDB.Driver;
using RateLimiter.Reader.Domain.Entities;
using RateLimiter.Reader.Domain.Interfaces;
using RateLimiter.Reader.Infrastructure.Data;

namespace RateLimiter.Reader.Application.Services;

public sealed class ReaderService : IReaderService
{
    private readonly IRateLimitRepository _repo;
    private readonly IMapperFactory _mapperFactory;
    private readonly ConcurrentDictionary<string, RateLimit> _limitsByRoute = new();
    private readonly ConcurrentDictionary<string, string> _idToRoute = new();
    private readonly object _initLock = new();
    private bool _initialized;
    private BsonDocument? _resumeToken;
    public ReaderService(IRateLimitRepository repo, IMapperFactory mapperFactory)
    {
        _repo = repo;
        _mapperFactory = mapperFactory;
    }

    public async Task InitializeAsync()
    {
        lock (_initLock)
        {
            if (_initialized) return;
            _initialized = true;
        }
        await foreach (var db in _repo.StreamAllAsync())
            ApplyUpsert(db);
    }

    public async Task WatchAsync()
    {
        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<RateLimitDbModel>>()
            .Match(x => 
                           x.OperationType == ChangeStreamOperationType.Insert 
                        || x.OperationType == ChangeStreamOperationType.Replace
                        || x.OperationType == ChangeStreamOperationType.Update
                        || x.OperationType == ChangeStreamOperationType.Delete);

        await foreach (var change in _repo.WatchAsync(_resumeToken, pipeline))
        {
            _resumeToken = change.ResumeToken;
            switch (change.OperationType)
            {
                case ChangeStreamOperationType.Insert:
                case ChangeStreamOperationType.Replace:
                case ChangeStreamOperationType.Update:
                {
                    var full = change.FullDocument ?? await ReloadAsync(change.DocumentKey);
                    if (full is not null) ApplyUpsert(full);
                    break;
                }
                case ChangeStreamOperationType.Delete:
                {
                    var idKey = change.DocumentKey["_id"].AsObjectId.ToString();
                    if (_idToRoute.TryRemove(idKey, out var oldRoute))
                        _limitsByRoute.TryRemove(oldRoute, out _);
                    break;
                }
            }
        }
    }
    public RateLimit[] GetAllLimits() => _limitsByRoute.Values.ToArray();

    private Task<RateLimitDbModel?> ReloadAsync(BsonDocument documentKey)
    {
        var oid = documentKey["_id"].AsObjectId;
        return _repo.GetByIdAsync(oid);
    }

    private void ApplyUpsert(RateLimitDbModel db)
    {
        var mapped = _mapperFactory.RateLimit.ToDomain(db);
        var idKey = db.Id;
        if (_idToRoute.TryGetValue(idKey, out var oldRoute) && oldRoute != mapped.Route)
            _limitsByRoute.TryRemove(oldRoute, out _);
        _idToRoute[idKey] = mapped.Route;
        _limitsByRoute[mapped.Route] = mapped;
    }
}
