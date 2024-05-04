using Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Services;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        collection.AddSingleton<IUserService, UserService>();
        collection.AddSingleton<IAccountService, AccountService>();
        return collection;
    }
}