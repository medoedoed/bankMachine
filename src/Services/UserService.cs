using Interfaces.Repositories;
using Interfaces.Services;
using Models;
using Models.Results;

namespace Services;

#pragma warning disable CA2007

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        ArgumentNullException.ThrowIfNull(userRepository);
        _userRepository = userRepository;
    }

    public async Task<UserResult> Login(string username, string password)
    {
        User? user = await _userRepository.FindUserByUsername(username);

        if (user is null) return new UserResult.Failure("User not found");
        if (user.Password != password) return new UserResult.Failure("Wrong password");
        return new UserResult.Success(user);
    }

    public async Task<StringResult> Register(string username, string password, string role)
    {
        User? user = await _userRepository.FindUserByUsername(username);
        if (user is not null) return new StringResult.Failure($"User {username} already exists");
        await _userRepository.CreateUser(username, password, role);
        return new StringResult.Success("Sign up successful");
    }
}