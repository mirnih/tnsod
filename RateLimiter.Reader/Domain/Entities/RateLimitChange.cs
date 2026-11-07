namespace RateLimiter.Reader.Domain.Entities;

public sealed record RateLimitChange(
    ChangeOperationType OperationType,
    RateLimit? RateLimit,
    string? DeletedId
);
