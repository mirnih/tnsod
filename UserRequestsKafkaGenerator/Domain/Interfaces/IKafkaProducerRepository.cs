using UserRequestsKafkaGenerator.Transport;

namespace UserRequestsKafkaGenerator.Domain.Interfaces;

public interface IKafkaProducerRepository : IAsyncDisposable
{
    Task ProduceAsync(KafkaEvent evt, CancellationToken ct);
}