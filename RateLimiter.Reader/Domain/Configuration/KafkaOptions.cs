namespace RateLimiter.Reader.Domain.Configuration;

public sealed class KafkaOptions
{
    public const string SectionName = "Kafka";
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string Topic { get; set; } = "user_requests";
    public string GroupId { get; set; } = "rate-limiter-reader-group";
    public bool EnableAutoCommit { get; set; } = true;
    public int AutoCommitIntervalMs { get; set; } = 5000;
}
