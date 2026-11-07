namespace ConsoleApp1.Domain.Interfaces;

public interface IUserBanRepository
{
    Task<bool> IsUserBannedAsync(string userId, string endpoint, CancellationToken cancellationToken );
}
