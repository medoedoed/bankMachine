using Interfaces.Repositories;
using Interfaces.Services;
using Models;
using Npgsql;

#pragma warning disable CA2007

namespace DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IConnectionProvider _connectionProvider;

    public UserRepository(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<User?> FindUserByUsername(string username)
    {
        const string sql = """
                            select *
                           from users
                           where user_name = ($1);
                           """;

        await using NpgsqlCommand command = _connectionProvider.NpgsqlDataSource.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue(username);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync() is false)
            return null;

        return new User(
            Id: reader.GetInt64(0),
            Username: reader.GetString(1),
            Password: reader.GetString(2),
            Role: reader.GetString(3));
    }

    public async Task CreateUser(string username, string password, string role)
    {
        const string sql = """
                           insert into users (user_name, password, role)
                           values (($1), ($2), ($3));
                           """;
        await using NpgsqlCommand command = _connectionProvider.NpgsqlDataSource.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue(username);
        command.Parameters.AddWithValue(password);
        command.Parameters.AddWithValue(role);

        await command.ExecuteNonQueryAsync();
    }
}