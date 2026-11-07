namespace ConsoleApp1.Domain.Configuration;

public sealed class RedisOptions
{
    public const string SectionName = "Redis";
    public string ConnectionString { get; set; } = "localhost:6379";
}
