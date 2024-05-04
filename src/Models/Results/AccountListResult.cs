namespace Models.Results;

#pragma warning disable CA1034

public abstract record AccountListResult
{
    private AccountListResult() { }

    public sealed record Success(IEnumerable<Account> Accounts) : AccountListResult;
    public sealed record Failure(string Message) : AccountListResult;
}

#pragma warning restore CA1034