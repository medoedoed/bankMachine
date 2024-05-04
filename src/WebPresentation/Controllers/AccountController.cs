using System.Globalization;
using System.Security.Claims;
using Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Results;

namespace WebPresentation.Controllers;

#pragma warning disable CA2007

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        ArgumentNullException.ThrowIfNull(accountService);
        _accountService = accountService;
    }

    [HttpPost("create-account")]
    public async Task<ActionResult> CreateAccount(string pin)
    {
        await _accountService.CreateAccount(GetUserId(), pin);
        return Ok();
    }

    [HttpGet("user-accounts")]
    public async Task<ActionResult<IEnumerable<Account>>> FindAccountsByUserId()
    {
        AccountListResult result = await _accountService.FindAccountsByUserId(GetUserId());
        if (result is AccountListResult.Success successResult) return Ok(successResult.Accounts.Select(account => account.AccountId));
        return BadRequest();
    }

    [HttpGet("account-balance")]
    public async Task<ActionResult<long>> GetAccountBalance(long accountId)
    {
        AccountResult result = await _accountService.FindAccountById(accountId);
        if (result is not AccountResult.Success resultSuccess) return BadRequest();
        if (resultSuccess.Account.UserId != GetUserId()) return Forbid();
        return Ok(resultSuccess.Account.Balance);
    }

    [HttpPatch("withdraw-money")]
    public async Task<ActionResult<string>> WithdrawMoneyFromAccount(int accountId, string pin, int amount)
    {
        AccountResult result = await _accountService.FindAccountById(accountId);
        if (result is not AccountResult.Success resultSuccess) return BadRequest();
        if (resultSuccess.Account.UserId != GetUserId()) return Forbid();
        StringResult withdrawResult = await _accountService.WithdrawMoneyFromAccount(accountId, pin, amount);
        if (withdrawResult is not StringResult.Success withdrawSuccess) return BadRequest();
        return Ok(withdrawSuccess.Message);
    }

    [HttpPatch("deposit-money")]
    public async Task<ActionResult<string>> DepositMoneyToAccount(int accountId, string pin, int amount)
    {
        AccountResult result = await _accountService.FindAccountById(accountId);
        if (result is not AccountResult.Success resultSuccess) return BadRequest();
        if (resultSuccess.Account.UserId != GetUserId()) return Forbid();
        StringResult depositResult = await _accountService.DepositMoneyToAccount(accountId, pin, amount);
        if (depositResult is not StringResult.Success depositSuccess) return BadRequest();
        return Ok(depositSuccess.Message);
    }

    [HttpGet("account-operations")]
    public async Task<ActionResult<Transactions>> GetAccountOperations(long accountId)
    {
        TransactionResult result = await _accountService.GetOperation(accountId);
        if (result is not TransactionResult.Success resultSuccess) return BadRequest();
        return Ok(resultSuccess.Transactions);
    }

    private long GetUserId()
    {
        string id = HttpContext.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                    ?? throw new ArgumentException("exception");
        return long.Parse(id, CultureInfo.CurrentCulture);
    }
}