using RateLimiter.Writer.Domain.Entities;

namespace RateLimiter.Writer.Domain.Interfaces;

public interface IRateLimitService
{
    Task<bool> CreateLimitAsync(RateLimit limit, CancellationToken cancellationToken = default);
    Task<RateLimit?> GetLimitByRouteAsync(string route, CancellationToken cancellationToken = default);
    Task<bool> UpdateLimitAsync(RateLimit limit, CancellationToken cancellationToken = default);
    Task<bool> DeleteLimitAsync(string route, CancellationToken cancellationToken = default);
}