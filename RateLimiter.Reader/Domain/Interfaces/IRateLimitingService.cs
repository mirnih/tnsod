using RateLimiter.Reader.Transport;

namespace RateLimiter.Reader.Domain.Interfaces;

public interface IRateLimitingService
{
    Task ProcessRequestAsync(UserRequest request);
}
