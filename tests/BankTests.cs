using System.Threading.Tasks;
using Interfaces.Repositories;
using Models;
using Models.Results;
using NSubstitute;
using Services;
using Xunit;

namespace Itmo.ObjectOrientedProgramming.Lab5.Tests;

#pragma warning disable CA2007

public class BankTests
{
    private readonly IAccountRepository _accountRepository;
    private readonly AccountService _accountService;

    public BankTests()
    {
        _accountRepository = Substitute.For<IAccountRepository>();
        ITransactionRepository transactionRepository = Substitute.For<ITransactionRepository>();
        _accountService = new AccountService(_accountRepository, transactionRepository);
    }

    [Fact]
    public async Task SuccessWithdrawingTest()
    {
        var account = new Account(1, 1, 100, "1234");
        _accountRepository.FindAccountById(1).Returns(account);
        StringResult result = await _accountService.WithdrawMoneyFromAccount(1, "1234", 100);
        await _accountRepository.Received(0).UpdateAccount(account with { Balance = 0 });
        Assert.IsType<StringResult.Success>(result);
    }

    [Theory]
    [InlineData(1000, "1234")]
    public async Task FailureWithdrawingTest(int balance, string pin)
    {
        var account = new Account(1, 1, balance, pin);
        _accountRepository.FindAccountById(1).Returns(account);
        StringResult result = await _accountService.WithdrawMoneyFromAccount(1, "1235", 1000);
        await _accountRepository.DidNotReceiveWithAnyArgs().UpdateAccount(Arg.Any<Account>());
        Assert.IsType<StringResult.Failure>(result);
    }

    [Fact]
    public async Task SuccessDepositingTest()
    {
        var account = new Account(1, 1, 0, "1234");
        _accountRepository.FindAccountById(1).Returns(account);
        StringResult result = await _accountService.DepositMoneyToAccount(1, "1234", 100);
        await _accountRepository.Received(0).UpdateAccount(account with { Balance = 100 });
        Assert.IsType<StringResult.Success>(result);
    }
}