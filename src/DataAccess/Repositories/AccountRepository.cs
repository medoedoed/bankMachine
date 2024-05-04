using Interfaces.Repositories;
using Interfaces.Services;
using Models;
using Npgsql;

#pragma warning disable CA2007

namespace DataAccess.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly IConnectionProvider _connectionProvider;

    public AccountRepository(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<Account?> FindAccountById(long accountId)
    {
        const string sql = """
                            select account_id, user_id, balance, pin
                           from accounts
                           where account_id = ($1);
                           """;
        await using NpgsqlCommand command = _connectionProvider.NpgsqlDataSource.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue(accountId);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync() is false)
            return null;

        return new Account(
            AccountId: reader.GetInt64(0),
            UserId: reader.GetInt64(1),
            Balance: reader.GetInt64(2),
            PinCode: reader.GetString(3));
    }

    public async Task<IEnumerable<Account>> FindAllAccountsByUserId(long userId)
    {
        const string sql = """
                            select account_id, balance, pin
                           from accounts
                           where user_id = ($1);
                           """;
        await using NpgsqlCommand command = _connectionProvider.NpgsqlDataSource.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue(userId);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

        var accounts = new List<Account>();

        if (await reader.ReadAsync() is false)
            return accounts;

        do
        {
            accounts.Add(new Account(
                AccountId: reader.GetInt64(0),
                UserId: userId,
                Balance: reader.GetInt64(1),
                PinCode: reader.GetString(2)));
        }
        while (await reader.ReadAsync());

        return accounts;
    }

    public async Task CreateAccount(long userId, string pin)
    {
        const string sql = """
                           insert into accounts (user_id, balance, pin)
                           values (($1), 0, ($2))
                           """;
        await using NpgsqlCommand command = _connectionProvider.NpgsqlDataSource.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue(userId);
        command.Parameters.AddWithValue(pin);

        await command.ExecuteNonQueryAsync();
    }

    public async Task Withdrawal(long accountId, long amount)
    {
        const string sql = """
                           update accounts
                           set balance = balance - ($2)
                           where account_id = ($1)
                           """;
        await using NpgsqlCommand command = _connectionProvider.NpgsqlDataSource.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue(accountId);
        command.Parameters.AddWithValue(amount);

        await command.ExecuteNonQueryAsync();
    }

    public async Task Deposit(long accountId, long amount)
    {
        const string sql = """
                           update accounts
                           set balance = balance + ($2)
                           where account_id = ($1)
                           """;
        await using NpgsqlCommand command = _connectionProvider.NpgsqlDataSource.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue(accountId);
        command.Parameters.AddWithValue(amount);

        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAccount(long accountId)
    {
        const string sql = """
                           delete 
                           from accounts
                           where account_id = ($1)
                           """;
        await using NpgsqlCommand command = _connectionProvider.NpgsqlDataSource.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue(accountId);

        await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateAccount(Account account)
    {
        ArgumentNullException.ThrowIfNull(account);

        const string sql = """
                             update accounts
                             set (user_id, balance, pin) = (($1), ($2), ($3))
                             where id = ($4)
                             """;

        await using NpgsqlCommand command = _connectionProvider.NpgsqlDataSource.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue(account.UserId);
        command.Parameters.AddWithValue(account.Balance);
        command.Parameters.AddWithValue(account.PinCode);
        command.Parameters.AddWithValue(account.UserId);

        await command.ExecuteNonQueryAsync();
    }
}