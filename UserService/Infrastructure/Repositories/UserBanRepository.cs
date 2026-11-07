using ConsoleApp1.Domain.Interfaces;
using StackExchange.Redis;

namespace ConsoleApp1.Infrastructure.Repositories;

public class UserBanRepository : IUserBanRepository
{
    private readonly IDatabase _redis;

    public UserBanRepository(IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
    }

    public Task<bool> IsUserBannedAsync(string userId, string endpoint, CancellationToken cancellationToken = default)
    {
        var banKey = $"ban:{userId}:{endpoint}";
        return _redis.KeyExistsAsync(banKey);
    }
}
