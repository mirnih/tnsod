using RateLimiter.Reader.Domain.Interfaces;
namespace RateLimiter.Reader.Hosted;

public sealed class ReaderHostedService : BackgroundService
{
    private readonly IReaderService _reader;
    public ReaderHostedService(IReaderService reader, ILogger<ReaderHostedService> logger)
    {
        _reader = reader;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _reader.InitializeAsync();
            await _reader.WatchAsync();
        }
        catch (OperationCanceledException)
        {
        }
    }
}