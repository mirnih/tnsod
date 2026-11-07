namespace RateLimiter.Reader.Domain.Interfaces;

public interface IKafkaConsumerService
{
    Task ConsumeAsync(CancellationToken cancellationToken);
}
