using Interfaces.Repositories;
using Interfaces.Services;
using Models;
using Models.Results;

namespace Services;

#pragma warning disable CA2007

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;

    public AccountService(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository)
    {
        ArgumentNullException.ThrowIfNull(accountRepository);
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<AccountListResult> FindAccountsByUserId(long userId)
    {
        IEnumerable<Account> accounts = await _accountRepository.FindAllAccountsByUserId(userId);
        var accountsList = accounts.ToList();
        if (accountsList.Count == 0) return new AccountListResult.Failure("No accounts found");
        return new AccountListResult.Success(accountsList);
    }

    public async Task<AccountResult> FindAccountById(long accountId)
    {
        Account? account = await _accountRepository.FindAccountById(accountId);
        if (account is null) return new AccountResult.Failure("Account not found");
        return new AccountResult.Success(account);
    }

    public async Task<StringResult> CreateAccount(long userId, string pin)
    {
        ArgumentNullException.ThrowIfNull(pin);
        if (pin.Length != 4) return new StringResult.Failure("Invalid pin code");
        if (pin.Any(c => !char.IsDigit(c))) return new StringResult.Failure("Invalid pin code");

        await _accountRepository.CreateAccount(userId, pin);
        return new StringResult.Success("Account created successfully");
    }

    public async Task<StringResult> WithdrawMoneyFromAccount(long accountId, string pin, int amount)
    {
        Account? account = await _accountRepository.FindAccountById(accountId);
        if (account is null) return new StringResult.Failure("Account not found");
        if (account.PinCode != pin) return new StringResult.Failure("Wrong password");
        if (account.Balance < amount) return new StringResult.Failure("Insufficient funds");
        await _accountRepository.Withdrawal(accountId, amount);
        await _transactionRepository.CreateOperation(account.UserId, accountId, -amount);
        return new StringResult.Success("Withdrawal successful");
    }

    public async Task<StringResult> DepositMoneyToAccount(long accountId, string pin, int amount)
    {
        Account? account = await _accountRepository.FindAccountById(accountId);
        if (account is null) return new StringResult.Failure("Account not found");
        if (account.PinCode != pin) return new StringResult.Failure("Wrong password");
        await _accountRepository.Deposit(accountId, amount);
        await _transactionRepository.CreateOperation(account.UserId, accountId, amount);
        return new StringResult.Success("Replenishment successful");
    }

    public async Task<TransactionResult> GetOperation(long accountId)
    {
        return await _transactionRepository.GetOperation(accountId);
    }
}