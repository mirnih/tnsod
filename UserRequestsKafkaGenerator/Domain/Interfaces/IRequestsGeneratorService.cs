using UserRequestsKafkaGenerator.Domain.Entities;

namespace UserRequestsKafkaGenerator.Domain.Interfaces;

public interface IRequestsGeneratorService
{
    int AddTask(int userId, string endpoint, int rpm);
    bool UpdateTaskRpm(int id, int rpm);
    bool UpdateTaskEndpoint(int id, string endpoint);
    bool RemoveTask(int id);
    IReadOnlyCollection<UserRequestTask> List();
}