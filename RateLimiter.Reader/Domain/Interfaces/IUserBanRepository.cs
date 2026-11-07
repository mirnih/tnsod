namespace RateLimiter.Reader.Domain.Interfaces;

public interface IUserBanRepository
{
    Task<bool> IsUserBannedAsync(int userId, string endpoint);
    Task BanUserAsync(int userId, string endpoint, TimeSpan duration);
}
