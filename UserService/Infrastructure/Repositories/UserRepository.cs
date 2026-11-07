using ConsoleApp1.Domain.Configuration;
using ConsoleApp1.Domain.Entities;
using ConsoleApp1.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Dapper;
using Npgsql;

namespace ConsoleApp1.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IOptions<DbConnectionOptions> dbOptions)
    {
        _connectionString = dbOptions.Value.DefaultConnection;
    }

    public async Task<int> CreateUserAsync(IUser user, CancellationToken cancellationToken )
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        
        var parameters = new DynamicParameters();
        parameters.Add("_login", user.Login);
        parameters.Add("_password", user.Password);
        parameters.Add("_name", user.Name);
        parameters.Add("_surname", user.Surname);
        parameters.Add("_age", user.Age);

        var newUserId = new CommandDefinition(
            "SELECT create_user(@_login, @_password, @_name, @_surname, @_age)",
            parameters, cancellationToken: cancellationToken
        );
        return await connection.ExecuteScalarAsync<int>(newUserId);
    }
    
    public async Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken )
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("_id", id);
        
        var deleteUserId = new CommandDefinition(
            "SELECT delete_user(@_id)",
            parameters, cancellationToken: cancellationToken
        );
        return await connection.ExecuteScalarAsync<bool>(deleteUserId);
    }

    public async Task<IUser?> GetByIdAsync(int id, CancellationToken cancellationToken )
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        var parameters = new DynamicParameters();
        parameters.Add("_id", id);

        var getUser = new CommandDefinition(
            "SELECT * FROM get_user_by_id(@_id)",
            parameters, cancellationToken: cancellationToken
        );
        return await connection.QueryFirstOrDefaultAsync<User?>(getUser);
    }

    public async Task<IUser[]> GetByNameAndSurnameAsync(string name, string surname, CancellationToken cancellationToken )
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("_name", name);
        parameters.Add("_surname", surname);

        var command = new CommandDefinition(
            "SELECT * FROM get_user_by_name_surname(@_name, @_surname)",
            parameters, cancellationToken: cancellationToken);
        var getArrayUser = await connection.QueryAsync<User>(command);
        return getArrayUser.ToArray();
    }

    public async Task<bool> UpdateUserAsync(IUser user, CancellationToken cancellationToken )
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("_name", user.Name);
        parameters.Add("_surname", user.Surname);
        parameters.Add("_age", user.Age);
        parameters.Add("_id", user.Id);
        parameters.Add("_password", user.Password);

        var updateUser = new CommandDefinition(
            "Select update_user(@_id, @_name, @_surname, @_password, @_age)", 
            parameters, cancellationToken: cancellationToken
        );
        return await connection.ExecuteScalarAsync<bool>(updateUser);
    }
}