namespace RateLimiter.Reader.Domain.Entities;

public sealed record RateLimit(
    string Id,
    string Route,
    int RequestsPerMinute
);