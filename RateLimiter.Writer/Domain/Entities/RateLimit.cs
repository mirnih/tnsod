namespace RateLimiter.Writer.Domain.Entities;

public class RateLimit
{
    public required string Route { get; set; }
    public int RequestsPerMinute { get; set; }
}