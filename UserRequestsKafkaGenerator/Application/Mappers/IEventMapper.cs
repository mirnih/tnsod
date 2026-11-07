using UserRequestsKafkaGenerator.Domain.Entities;
using UserRequestsKafkaGenerator.Transport;
namespace UserRequestsKafkaGenerator.Application.Mappers;

public interface IEventMapper
{
    KafkaEvent ToKafkaEvent(UserRequestTask source);
}