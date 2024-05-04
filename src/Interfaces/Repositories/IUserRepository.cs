using Models;

namespace Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> FindUserByUsername(string username);
    Task CreateUser(string username, string password, string role);
}