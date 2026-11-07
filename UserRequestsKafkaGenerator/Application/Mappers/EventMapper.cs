using UserRequestsKafkaGenerator.Domain.Entities;
using UserRequestsKafkaGenerator.Transport;
namespace UserRequestsKafkaGenerator.Application.Mappers;

public sealed class EventMapper : IEventMapper
{
    public KafkaEvent ToKafkaEvent(UserRequestTask source) => new(source.UserId, source.Endpoint);
}