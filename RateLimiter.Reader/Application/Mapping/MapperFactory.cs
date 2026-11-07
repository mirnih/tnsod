using RateLimiter.Reader.Domain.Interfaces;

namespace RateLimiter.Reader.Application.Mapping;

public sealed class MapperFactory : IMapperFactory
{
    public MapperFactory(IRateLimitMapper rateLimit) => RateLimit = rateLimit;
    public IRateLimitMapper RateLimit { get; }
}