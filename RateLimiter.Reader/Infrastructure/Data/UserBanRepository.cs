using Microsoft.Extensions.Options;
using RateLimiter.Reader.Domain.Configuration;
using RateLimiter.Reader.Domain.Interfaces;
using StackExchange.Redis;

namespace RateLimiter.Reader.Infrastructure.Data;

public sealed class UserBanRepository : IUserBanRepository
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    public UserBanRepository(IOptions<RedisOptions> options)
    {
        _redis = ConnectionMultiplexer.Connect(options.Value.ConnectionString);
        _database = _redis.GetDatabase();
    }

    public async Task<bool> IsUserBannedAsync(int userId, string endpoint)
    {
        var key = GetBanKey(userId, endpoint);
        return await _database.KeyExistsAsync(key);
    }

    public async Task BanUserAsync(int userId, string endpoint, TimeSpan duration)
    {
        var key = GetBanKey(userId, endpoint);
        await _database.StringSetAsync(key, "1", duration);
    }

    private static string GetBanKey(int userId, string endpoint)
        => $"ban:{userId}:{endpoint}";
}
