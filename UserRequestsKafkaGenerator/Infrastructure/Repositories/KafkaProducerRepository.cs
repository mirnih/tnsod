using System.Text.Json;
using System.Text.Json.Serialization;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using UserRequestsKafkaGenerator.Domain.Configuration;
using UserRequestsKafkaGenerator.Domain.Interfaces;
using UserRequestsKafkaGenerator.Transport;

namespace UserRequestsKafkaGenerator.Infrastructure.Repositories;

public sealed class KafkaProducerRepository : IKafkaProducerRepository
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _topic;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public KafkaProducerRepository(IOptions<KafkaOptions> options)
    {
        var cfg = options.Value;
        _topic = cfg.Topic;
        var pc = new ProducerConfig
        {
            BootstrapServers = cfg.BootstrapServers,
            Acks = Acks.Leader,
            AllowAutoCreateTopics = cfg.AllowAutoCreateTopics,
            MessageTimeoutMs = cfg.MessageTimeoutMs,
            CompressionType = CompressionType.Lz4,
            EnableIdempotence = false
        };
        _producer = new ProducerBuilder<Null, string>(pc).Build();
    }

    public async Task ProduceAsync(KafkaEvent evt, CancellationToken ct)
    {
        var payload = JsonSerializer.Serialize(evt, JsonOptions);
        await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = payload }, ct);
    }

    public ValueTask DisposeAsync()
    {
        try { _producer.Flush(TimeSpan.FromSeconds(5)); } catch { }
        _producer.Dispose();
        return ValueTask.CompletedTask;
    }
}