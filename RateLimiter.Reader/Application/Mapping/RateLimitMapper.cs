using Riok.Mapperly.Abstractions;
using RateLimiter.Reader.Domain.Entities;
using RateLimiter.Reader.Domain.Interfaces;
using RateLimiter.Reader.Infrastructure.Data;

namespace RateLimiter.Reader.Application.Mapping;

[Mapper]
public partial class RateLimitMapper : IRateLimitMapper
{
    public partial RateLimit ToDomain(RateLimitDbModel db);
    public partial RateLimitDbModel ToDb(RateLimit domain);
}