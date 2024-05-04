using Models;

namespace Interfaces.Repositories;

public interface IAccountRepository
{
    Task<Account?> FindAccountById(long accountId);
    Task<IEnumerable<Account>> FindAllAccountsByUserId(long userId);
    Task CreateAccount(long userId, string pin);
    Task Withdrawal(long accountId, long amount);
    Task Deposit(long accountId, long amount);
    Task DeleteAccount(long accountId);
    Task UpdateAccount(Account account);
}