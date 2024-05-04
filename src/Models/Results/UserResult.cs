namespace Models.Results;

#pragma warning disable CA1034

public abstract record UserResult
{
    private UserResult() { }

    public sealed record Success(User User) : UserResult;

    public sealed record Failure(string Message) : UserResult;
}

#pragma warning restore CA1034