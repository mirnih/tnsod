using RateLimiter.Reader.Domain.Entities;

namespace RateLimiter.Reader.Domain.Interfaces;

public interface IReaderService
{
    Task InitializeAsync();
    Task WatchAsync();
    RateLimit[] GetAllLimits();
}