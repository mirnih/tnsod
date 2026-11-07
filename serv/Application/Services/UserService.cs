using ConsoleApp1.Domain.Entities;
using ConsoleApp1.Domain.Interfaces;

namespace ConsoleApp1.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<int> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        var result = await _userRepository.CreateUserAsync(user, cancellationToken);
        if (result == 0) throw new Exception("Пользователь с таким логином уже существует.");
        return result;
    }

    public Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _userRepository.GetByIdAsync(id, cancellationToken);
    }
    
    public async Task<User[]> GetByNameAndSurnameAsync(string name, string surname, CancellationToken cancellationToken = default)
    {
        var result = await _userRepository.GetByNameAndSurnameAsync(name, surname, cancellationToken);
        if (result.Length == 0) throw new Exception("Пользователь с таким именем не существует.");
        return result;
    }
    
    public async Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        var result = await _userRepository.UpdateUserAsync(user, cancellationToken);
        if (result == false) throw new Exception("Пользователь с таким id не существует.");
        return result;
    }
    
    public async Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = await _userRepository.DeleteUserAsync(id, cancellationToken);
        if (result == false) throw new Exception("Пользователь с таким id не существует.");
        return result;
    }
}