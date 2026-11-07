namespace ConsoleApp1.Domain.Interfaces;

public interface IUserBanService
{
    Task<bool> IsUserBannedAsync(string userId, string endpoint, CancellationToken cancellationToken );
}
