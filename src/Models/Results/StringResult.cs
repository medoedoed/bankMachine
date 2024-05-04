namespace Models.Results;

#pragma warning disable CA1034

public abstract record StringResult
{
    private StringResult() { }

    public sealed record Success(string Message) : StringResult;

    public sealed record Failure(string Message) : StringResult;
}

#pragma warning restore CA1034