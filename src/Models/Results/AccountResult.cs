namespace Models.Results;

#pragma warning disable CA1034

public abstract record AccountResult
{
    private AccountResult() { }

    public sealed record Success(Account Account) : AccountResult;
    public sealed record Failure(string Message) : AccountResult;
}

#pragma warning restore CA1034