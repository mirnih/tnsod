namespace UserRequestsKafkaGenerator.Domain.Entities;

public sealed class UserRequestTask
{
    public int Id { get; }
    public int UserId { get; private set; }
    public string Endpoint { get; private set; }
    public int Rpm { get; private set; }
    public readonly CancellationTokenSource Cts = new();

    public UserRequestTask(int id, int userId, string endpoint, int rpm)
    {
        Id = id;
        UserId = userId;
        Endpoint = endpoint;
        Rpm = rpm;
    }

    public void Update(int? rpm = null, string? endpoint = null)
    {
        if (rpm.HasValue && rpm.Value > 0) Rpm = rpm.Value;
        if (!string.IsNullOrWhiteSpace(endpoint)) Endpoint = endpoint!;
    }
}