namespace ConsoleApp1.Domain.Interfaces;

public interface IUserRepository
{
    Task<int> CreateUserAsync(IUser user, CancellationToken cancellationToken );
    Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken );
    Task<IUser?> GetByIdAsync(int id, CancellationToken cancellationToken );
    Task<IUser[]> GetByNameAndSurnameAsync(string name, string surname, CancellationToken cancellationToken );
    Task<bool> UpdateUserAsync(IUser user, CancellationToken cancellationToken );
}