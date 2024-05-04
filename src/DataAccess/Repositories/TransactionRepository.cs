using Interfaces.Repositories;
using Interfaces.Services;
using Models;
using Models.Results;
using Npgsql;

#pragma warning disable CA2007

namespace DataAccess.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly IConnectionProvider _connectionProvider;

    public TransactionRepository(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task CreateOperation(long userId, long accountId, long amount)
    {
        const string sql = """
                           insert into operations
                           (user_id, account_id, amount)
                           values
                           (($1), ($2), ($3))
                           """;
        await using NpgsqlCommand command = _connectionProvider.NpgsqlDataSource.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue(userId);
        command.Parameters.AddWithValue(accountId);
        command.Parameters.AddWithValue(amount);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<TransactionResult> GetOperation(long accountId)
    {
        const string sql = """
                           select amount
                           from operations
                           where account_id = ($1)
                           """;
        await using NpgsqlCommand command = _connectionProvider.NpgsqlDataSource.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue(accountId);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

        List<long> amounts = new();

        while (await reader.ReadAsync())
        {
            amounts.Add(reader.GetInt64(0));
        }

        if (amounts.Count == 0) return new TransactionResult.Failure("No operations found");

        return new TransactionResult.Success(new Transactions(accountId, amounts));
    }
}