using RateLimiter.Reader.Domain.Interfaces;

namespace RateLimiter.Reader.Hosted;

public sealed class KafkaConsumerHostedService : BackgroundService
{
    private readonly IKafkaConsumerService _consumerService;
    private readonly ILogger<KafkaConsumerHostedService> _logger;

    public KafkaConsumerHostedService(
        IKafkaConsumerService consumerService,
        ILogger<KafkaConsumerHostedService> logger)
    {
        _consumerService = consumerService;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("KafkaConsumerHostedService is starting");

        return Task.Factory.StartNew(
            async () =>
            {
                try
                {
                    await _consumerService.ConsumeAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("KafkaConsumerHostedService is stopping");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in KafkaConsumerHostedService");
                    throw;
                }
            },
            stoppingToken,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default).Unwrap();
    }
}
