namespace RateLimiter.Reader.Domain.Interfaces
{
    public interface IMapperFactory
    {
        IRateLimitMapper RateLimit { get; }
    }
}