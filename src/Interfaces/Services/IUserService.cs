using Models.Results;

namespace Interfaces.Services;

public interface IUserService
{
    Task<UserResult> Login(string username, string password);
    Task<StringResult> Register(string username, string password, string role);
}