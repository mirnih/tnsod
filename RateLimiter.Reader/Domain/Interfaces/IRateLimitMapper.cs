using RateLimiter.Reader.Domain.Entities;
using RateLimiter.Reader.Infrastructure.Data;

namespace RateLimiter.Reader.Domain.Interfaces;

public interface IRateLimitMapper
{
    RateLimit ToDomain(RateLimitDbModel db);
    RateLimitDbModel ToDb(RateLimit domain);
}