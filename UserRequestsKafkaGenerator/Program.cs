using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using UserRequestsKafkaGenerator.Application.Mappers;
using UserRequestsKafkaGenerator.Application.Services;
using UserRequestsKafkaGenerator.Domain.Configuration;
using UserRequestsKafkaGenerator.Domain.Interfaces;
using UserRequestsKafkaGenerator.Infrastructure.Repositories;
var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection(KafkaOptions.SectionName));
builder.Services.AddSingleton<IKafkaProducerRepository, KafkaProducerRepository>();
builder.Services.AddSingleton<IEventMapper, EventMapper>();
builder.Services.AddSingleton<IRequestsGeneratorService, RequestsGeneratorService>();
var app = builder.Build();
Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

void PrintHelp()
{
    Console.WriteLine("Commands:");
    Console.WriteLine("  add <userId> <endpoint> <rpm>");
    Console.WriteLine("  update <id> rpm <value>");
    Console.WriteLine("  update <id> endpoint <value>");
    Console.WriteLine("  remove <id>");
    Console.WriteLine("  list");
    Console.WriteLine("  exit");
}

using var scope = app.Services.CreateScope();
var svc = scope.ServiceProvider.GetRequiredService<IRequestsGeneratorService>();
var kafka = scope.ServiceProvider.GetRequiredService<IOptions<KafkaOptions>>().Value;

Console.WriteLine($"Kafka: {kafka.BootstrapServers}, topic: {kafka.Topic}");
PrintHelp();

while (true)
{
    Console.Write("> ");
    var line = Console.ReadLine();
    if (line is null) continue;
    var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    if (parts.Length == 0) continue;

    var cmd = parts[0].ToLowerInvariant();

    if (cmd is "exit" ) break;
    
    if (cmd == "add" && parts.Length >= 4 && int.TryParse(parts[1], out var userId))
    {
        var endpoint = parts[2];
        if (!int.TryParse(parts[3], out var rpm) || rpm <= 0 || userId <= 0 || string.IsNullOrWhiteSpace(endpoint))
        {
            Console.WriteLine("invalid args");
            continue;
        }
        var id = svc.AddTask(userId, endpoint, rpm);
        Console.WriteLine(id > 0 ? $"added id={id}" : "failed");
        continue;
    }

    if (cmd == "update" && parts.Length >= 4 && int.TryParse(parts[1], out var updId))
    {
        if (parts[2].Equals("rpm", StringComparison.OrdinalIgnoreCase))
        {
            if (!int.TryParse(parts[3], out var newRpm) || newRpm <= 0)
            {
                Console.WriteLine("invalid rpm");
                continue;
            }
            Console.WriteLine(svc.UpdateTaskRpm(updId, newRpm) ? $"updated id={updId} rpm={newRpm}" : "not found");
            continue;
        }

        if (parts[2].Equals("endpoint", StringComparison.OrdinalIgnoreCase))
        {
            var newEndpoint = string.Join(' ', parts.Skip(3));
            if (string.IsNullOrWhiteSpace(newEndpoint))
            {
                Console.WriteLine("invalid endpoint");
                continue;
            }
            Console.WriteLine(svc.UpdateTaskEndpoint(updId, newEndpoint) ? $"updated id={updId} endpoint={newEndpoint}" : "not found");
            continue;
        }

        Console.WriteLine("unknown field");
        continue;
    }

    if (cmd == "remove" && parts.Length >= 2 && int.TryParse(parts[1], out var remId))
    {
        Console.WriteLine(svc.RemoveTask(remId) ? $"removed id={remId}" : "not found");
        continue;
    }

    if (cmd == "list")
    {
        var list = svc.List();
        foreach (var t in list) Console.WriteLine($"id={t.Id} user={t.UserId} endpoint={t.Endpoint} rpm={t.Rpm}");
        continue;
    }
}

if (svc is IAsyncDisposable d) await d.DisposeAsync();
