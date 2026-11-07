using ConsoleApp1.Domain.Entities;
namespace ConsoleApp1.Domain.Interfaces;

public interface IUserRepository
{
    Task<int> CreateUserAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<User[]> GetByNameAndSurnameAsync(string name, string surname, CancellationToken cancellationToken = default);
    Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken = default);
}