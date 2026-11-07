using ConsoleApp1.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace ConsoleApp1.Application.Services;

public class UserService : IUserService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    private readonly IUserRepository _userRepository;
    private readonly IMemoryCache _cache;

    public UserService(IUserRepository userRepository, IMemoryCache cache)
    {
        _userRepository = userRepository;
        _cache = cache;
    }

    public async Task<int> CreateUserAsync(IUser user, CancellationToken cancellationToken)
    {
        var newId = await _userRepository.CreateUserAsync(user, cancellationToken);
        if (newId == 0) throw new Exception("Пользователь с таким логином уже существует.");

        var created = await _userRepository.GetByIdAsync(newId, cancellationToken);
        if (created != null)
        {
            _cache.Set(GetUserCacheKey(newId), created, CacheTtl);
            TryUpdateUsersListCacheIfExists(created.Name, created.Surname, list => UpsertById(list, created));
        }

        return newId;
    }

    public async Task<IUser?> GetUserByIdAsync(int id, CancellationToken cancellationToken)
    {
        var cacheKey = GetUserCacheKey(id);

        if (_cache.TryGetValue(cacheKey, out IUser? cachedUser))
            return cachedUser;

        var user = await _userRepository.GetByIdAsync(id, cancellationToken);

        if (user != null)
            _cache.Set(cacheKey, user, CacheTtl);

        return user;
    }

    public async Task<IUser[]> GetByNameAndSurnameAsync(string name, string surname, CancellationToken cancellationToken)
    {
        var cacheKey = GetUsersByCacheKey(name, surname);

        if (_cache.TryGetValue(cacheKey, out IUser[]? cachedUsers))
            return cachedUsers!;

        var result = await _userRepository.GetByNameAndSurnameAsync(name, surname, cancellationToken);
        if (result.Length == 0) throw new Exception("Пользователь с таким именем не существует.");

        _cache.Set(cacheKey, result, CacheTtl);

        return result;
    }

    public async Task<bool> UpdateUserAsync(IUser user, CancellationToken cancellationToken)
    {
        var userKey = GetUserCacheKey(user.Id);
        _cache.TryGetValue(userKey, out IUser? beforeUpdate);

        var result = await _userRepository.UpdateUserAsync(user, cancellationToken);
        if (!result) throw new Exception("Пользователь с таким id не существует.");

        var updated = await _userRepository.GetByIdAsync(user.Id, cancellationToken);
        if (updated != null)
        {
            _cache.Set(userKey, updated, CacheTtl);

            if (beforeUpdate != null)
            {
                TryUpdateUsersListCacheIfExists(beforeUpdate.Name, beforeUpdate.Surname, list => RemoveById(list, user.Id));
            }

            TryUpdateUsersListCacheIfExists(updated.Name, updated.Surname, list => UpsertById(list, updated));
        }

        return result;
    }

    public async Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken)
    {
        var userKey = GetUserCacheKey(id);
    
        _cache.TryGetValue(userKey, out IUser? cachedUser);
        var userForList = cachedUser ?? await _userRepository.GetByIdAsync(id, cancellationToken);
    
        var result = await _userRepository.DeleteUserAsync(id, cancellationToken);
        if (!result) throw new Exception("Пользователь с таким id не существует.");
    
        _cache.Remove(userKey);
    
        if (userForList != null)
        {
            TryUpdateUsersListCacheIfExists(userForList.Name, userForList.Surname, list => RemoveById(list, id));
        }
    
        return result;
    }

    private void TryUpdateUsersListCacheIfExists(string name, string surname, Func<IUser[], IUser[]> updater)
    {
        var key = GetUsersByCacheKey(name, surname);
        if (!_cache.TryGetValue(key, out IUser[]? existing) || existing == null)
            return;

        var updated = updater(existing);
        _cache.Set(key, updated, CacheTtl);
    }

    private static IUser[] UpsertById(IUser[] users, IUser user)
    {
        var idx = Array.FindIndex(users, u => u.Id == user.Id);
        if (idx >= 0)
        {
            var copy = (IUser[])users.Clone();
            copy[idx] = user;
            return copy;
        }

        var expanded = new IUser[users.Length + 1];
        Array.Copy(users, expanded, users.Length);
        expanded[^1] = user;
        return expanded;
    }

    private static IUser[] RemoveById(IUser[] users, int id)
    {
        var idx = Array.FindIndex(users, u => u.Id == id);
        if (idx < 0) return users;

        if (users.Length == 1) return Array.Empty<IUser>();

        var result = new IUser[users.Length - 1];
        if (idx > 0) Array.Copy(users, 0, result, 0, idx);
        if (idx < users.Length - 1) Array.Copy(users, idx + 1, result, idx, users.Length - idx - 1);
        return result;
    }

    private static string GetUserCacheKey(int id) => $"user:{id}";

    private static string GetUsersByCacheKey(string name, string surname)
        => $"users:{name.Trim().ToLower()}:{surname.Trim().ToLower()}";
}
