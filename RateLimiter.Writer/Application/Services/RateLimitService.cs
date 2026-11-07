using RateLimiter.Writer.Domain.Entities;
using RateLimiter.Writer.Domain.Interfaces;
namespace RateLimiter.Writer.Application.Services;

public class RateLimitService : IRateLimitService
{
    private readonly IRateLimitRepository _repository;

    public RateLimitService(IRateLimitRepository repository)
    {
        _repository = repository;
    }

    public Task<bool> CreateLimitAsync(RateLimit limit, CancellationToken cancellationToken = default)
    {
        return _repository.CreateLimitAsync(limit, cancellationToken);
    }

    public Task<RateLimit?> GetLimitByRouteAsync(string route, CancellationToken cancellationToken = default)
    {
        return _repository.GetLimitByRouteAsync(route, cancellationToken);
    }

    public Task<bool> UpdateLimitAsync(RateLimit limit, CancellationToken cancellationToken = default)
    {
        return _repository.UpdateLimitAsync(limit, cancellationToken);
    }

    public Task<bool> DeleteLimitAsync(string route, CancellationToken cancellationToken = default)
    {
        return _repository.DeleteLimitAsync(route, cancellationToken);
    }
}