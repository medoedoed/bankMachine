using Npgsql;

namespace Interfaces.Services;

public interface IConnectionProvider
{
    NpgsqlDataSource NpgsqlDataSource { get; }
}