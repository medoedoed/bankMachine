using Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

#pragma warning disable CA2007

namespace DataAccess.Migrations;

public static class ServiceScopeExtensions
{
    public static void SetUpDataBase(this IServiceScope scope)
    {
        ArgumentNullException.ThrowIfNull(scope);
        const string query = """
                             create table if not exists users
                             (
                                 user_id bigint primary key generated always as identity,
                                 user_name text not null,
                                 password text not null,
                                 role text not null
                             );
                             
                             create table if not exists accounts
                             (
                                 account_id bigint primary key generated always as identity,
                                 user_id bigint not null references users (user_id),
                                 balance integer not null,
                                 pin text not null
                             );
                             
                             create table if not exists operations
                             (
                                 user_id bigint not null references users (user_id),
                                 account_id bigint not null references accounts (account_id),
                                 amount integer not null
                             );
                             
                             insert into users (user_name, password, role) values ('admin', 'admin', 'admin');
                             """;
        IConnectionProvider connectionProvider =
            scope.ServiceProvider.GetRequiredService<IConnectionProvider>();
        using NpgsqlCommand command = connectionProvider.NpgsqlDataSource.CreateCommand(query);
        command.ExecuteNonQuery();
    }

    public static void ResetDataBase(this IServiceScope scope)
    {
        ArgumentNullException.ThrowIfNull(scope);
        const string query = "drop table if exists operations, users, accounts";
        IConnectionProvider connectionProvider =
            scope.ServiceProvider.GetRequiredService<IConnectionProvider>();
        using NpgsqlCommand command = connectionProvider.NpgsqlDataSource.CreateCommand(query);
        command.ExecuteNonQuery();
    }
}