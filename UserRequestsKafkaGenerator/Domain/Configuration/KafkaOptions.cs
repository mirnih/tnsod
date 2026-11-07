namespace UserRequestsKafkaGenerator.Domain.Configuration;

public sealed class KafkaOptions
{
    public const string SectionName = "Kafka";
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string Topic { get; set; } = "user_requests";
    public int MessageTimeoutMs { get; set; } = 15000;
    public bool AllowAutoCreateTopics { get; set; } = true;
}