using Interfaces.Services;
using Npgsql;

namespace DataAccess.Connections;

public class ConnectionProvider : IConnectionProvider
{
    public ConnectionProvider(string connectionString)
    {
        NpgsqlDataSource = NpgsqlDataSource.Create(connectionString);
    }

    public NpgsqlDataSource NpgsqlDataSource { get; }
}