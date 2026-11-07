using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using RateLimiter.Reader.Domain.Configuration;
using RateLimiter.Reader.Domain.Interfaces;
using RateLimiter.Reader.Transport;

namespace RateLimiter.Reader.Application.Services;

public sealed class KafkaConsumerService : IKafkaConsumerService
{
    private readonly IRateLimitingService _rateLimitingService;
    private readonly KafkaOptions _kafkaOptions;
    private readonly ILogger<KafkaConsumerService> _logger;

    public KafkaConsumerService(
        IRateLimitingService rateLimitingService,
        IOptions<KafkaOptions> kafkaOptions,
        ILogger<KafkaConsumerService> logger)
    {
        _rateLimitingService = rateLimitingService;
        _kafkaOptions = kafkaOptions.Value;
        _logger = logger;
    }

    public async Task ConsumeAsync(CancellationToken cancellationToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaOptions.BootstrapServers,
            GroupId = _kafkaOptions.GroupId,
            EnableAutoCommit = _kafkaOptions.EnableAutoCommit,
            AutoCommitIntervalMs = _kafkaOptions.AutoCommitIntervalMs,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(_kafkaOptions.Topic);

        _logger.LogInformation("Kafka consumer started. Topic: {Topic}, GroupId: {GroupId}",
            _kafkaOptions.Topic, _kafkaOptions.GroupId);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(cancellationToken);

                    if (consumeResult?.Message?.Value is null)
                        continue;

                    var userRequest = JsonSerializer.Deserialize<UserRequest>(consumeResult.Message.Value);

                    if (userRequest is not null)
                        await _rateLimitingService.ProcessRequestAsync(userRequest);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from Kafka");
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Error deserializing message from Kafka");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka consumer is shutting down");
        }
        finally
        {
            consumer.Close();
        }
    }
}
