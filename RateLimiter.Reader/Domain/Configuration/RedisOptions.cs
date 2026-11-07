namespace RateLimiter.Reader.Domain.Configuration;

public sealed class RedisOptions
{
    public const string SectionName = "Redis";
    public string ConnectionString { get; set; } = "localhost:6379";
    public int BanDurationMinutes { get; set; } = 5;
}
