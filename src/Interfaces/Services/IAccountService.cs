using Models.Results;

namespace Interfaces.Services;

public interface IAccountService
{
    Task<AccountListResult> FindAccountsByUserId(long userId);
    Task<AccountResult> FindAccountById(long accountId);
    Task<StringResult> CreateAccount(long userId, string pin);
    Task<StringResult> WithdrawMoneyFromAccount(long accountId, string pin, int amount);
    Task<StringResult> DepositMoneyToAccount(long accountId, string pin, int amount);
    Task<TransactionResult> GetOperation(long accountId);
}