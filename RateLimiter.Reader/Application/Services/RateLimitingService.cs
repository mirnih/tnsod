using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using RateLimiter.Reader.Domain.Configuration;
using RateLimiter.Reader.Domain.Interfaces;
using RateLimiter.Reader.Transport;

namespace RateLimiter.Reader.Application.Services;

public sealed class RateLimitingService : IRateLimitingService
{
    private readonly IReaderService _readerService;
    private readonly IUserBanRepository _banRepository;
    private readonly RedisOptions _redisOptions;
    private readonly ConcurrentDictionary<(int userId, string endpoint), RequestTracker> _trackers = new();

    public RateLimitingService(
        IReaderService readerService,
        IUserBanRepository banRepository,
        IOptions<RedisOptions> redisOptions)
    {
        _readerService = readerService;
        _banRepository = banRepository;
        _redisOptions = redisOptions.Value;
    }

    public async Task ProcessRequestAsync(UserRequest request)
    {
        var limits = _readerService.GetAllLimits();
        var limit = limits.FirstOrDefault(l => l.Route == request.endpoint);

        if (limit is null)
            return;

        var isBanned = await _banRepository.IsUserBannedAsync(request.user_id, request.endpoint);
        if (isBanned)
            return;

        var tracker = _trackers.GetOrAdd((request.user_id, request.endpoint), _ => new RequestTracker());

        var now = DateTime.UtcNow;
        var shouldBan = false;

        lock (tracker.Timestamps)
        {
            tracker.Timestamps.RemoveAll(ts => (now - ts).TotalMinutes > 1);
            tracker.Timestamps.Add(now);

            if (tracker.Timestamps.Count > limit.RequestsPerMinute)
                shouldBan = true;
        }

        if (shouldBan)
        {
            var banDuration = TimeSpan.FromMinutes(_redisOptions.BanDurationMinutes);
            await _banRepository.BanUserAsync(request.user_id, request.endpoint, banDuration);
        }
    }

    private sealed class RequestTracker
    {
        public List<DateTime> Timestamps { get; } = new();
    }
}
