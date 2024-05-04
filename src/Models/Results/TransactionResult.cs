namespace Models.Results;

#pragma warning disable CA1034

public abstract record TransactionResult
{
    private TransactionResult() { }

    public sealed record Success(Transactions Transactions) : TransactionResult;

    public sealed record Failure(string Message) : TransactionResult;
}

#pragma warning restore CA1034