using Riok.Mapperly.Abstractions;
using RateLimiter.Writer.Domain.Entities;
namespace RateLimiter.Writer.Application.Mappers;

[Mapper]
public static partial class RateLimitMapper
{
    public static partial RateLimit ToRateLimit(CreateLimitRequest request);
    public static partial RateLimit ToRateLimit(UpdateLimitRequest request);
    public static partial RateLimitModel ToRateLimitModel(RateLimit rateLimit);
}