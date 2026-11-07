using RateLimiter.Reader.Domain.Entities;

namespace RateLimiter.Reader.Domain.Interfaces;

public interface IRateLimitRepository
{
    IAsyncEnumerable<RateLimit> StreamAllAsync(int batchSize = 1000);
    IAsyncEnumerable<RateLimitChange> WatchAsync();
}