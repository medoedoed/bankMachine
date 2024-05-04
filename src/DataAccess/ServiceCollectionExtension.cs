using DataAccess.Connections;
using DataAccess.Repositories;
using Interfaces.Repositories;
using Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddDataAccess(
        this IServiceCollection collection,
        string connectionString)
    {
        collection.AddSingleton<IConnectionProvider>(new ConnectionProvider(connectionString));
        collection.AddSingleton<IUserRepository, UserRepository>();
        collection.AddSingleton<IAccountRepository, AccountRepository>();
        collection.AddSingleton<ITransactionRepository, TransactionRepository>();

        return collection;
    }
}