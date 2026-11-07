using System.Collections.Concurrent;
using RateLimiter.Reader.Domain.Entities;
using RateLimiter.Reader.Domain.Interfaces;

namespace RateLimiter.Reader.Application.Services;

public sealed class ReaderService : IReaderService
{
    private readonly IRateLimitRepository _repo;
    private readonly ConcurrentDictionary<string, RateLimit> _limitsByRoute = new();
    private readonly ConcurrentDictionary<string, string> _idToRoute = new();
    private readonly object _initLock = new();
    private bool _initialized;

    public ReaderService(IRateLimitRepository repo)
    {
        _repo = repo;
    }

    public async Task InitializeAsync()
    {
        lock (_initLock)
        {
            if (_initialized) return;
            _initialized = true;
        }

        await foreach (var rateLimit in _repo.StreamAllAsync())
        {
            ApplyUpsert(rateLimit);
        }
    }

    public async Task WatchAsync()
    {
        await foreach (var change in _repo.WatchAsync())
        {
            switch (change.OperationType)
            {
                case ChangeOperationType.Insert:
                case ChangeOperationType.Replace:
                case ChangeOperationType.Update:
                {
                    if (change.RateLimit is not null)
                        ApplyUpsert(change.RateLimit);
                    break;
                }
                case ChangeOperationType.Delete:
                {
                    if (change.DeletedId is not null && _idToRoute.TryRemove(change.DeletedId, out var oldRoute))
                        _limitsByRoute.TryRemove(oldRoute, out _);
                    break;
                }
            }
        }
    }

    public RateLimit[] GetAllLimits() => _limitsByRoute.Values.ToArray();

    private void ApplyUpsert(RateLimit rateLimit)
    {
        var idKey = rateLimit.Id;
        if (_idToRoute.TryGetValue(idKey, out var oldRoute) && oldRoute != rateLimit.Route)
            _limitsByRoute.TryRemove(oldRoute, out _);
        _idToRoute[idKey] = rateLimit.Route;
        _limitsByRoute[rateLimit.Route] = rateLimit;
    }
}
