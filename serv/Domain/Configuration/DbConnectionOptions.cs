namespace ConsoleApp1.Domain.Configuration;

public class DbConnectionOptions
{
    public const string SectionName = "ConnectionStrings";
    public string DefaultConnection { get; set; } = string.Empty;
}