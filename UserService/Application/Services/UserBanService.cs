using ConsoleApp1.Domain.Interfaces;

namespace ConsoleApp1.Application.Services;

public class UserBanService : IUserBanService
{
    private readonly IUserBanRepository _userBanRepository;

    public UserBanService(IUserBanRepository userBanRepository)
    {
        _userBanRepository = userBanRepository;
    }

    public Task<bool> IsUserBannedAsync(string userId, string endpoint, CancellationToken cancellationToken )
    {
        return _userBanRepository.IsUserBannedAsync(userId, endpoint, cancellationToken);
    }
}
