namespace Models;

public class Transactions
{
    public Transactions(long accountId, IEnumerable<long> amounts)
    {
        AccountID = accountId;
        Amounts = amounts;
    }

    public long AccountID { get; }
    public IEnumerable<long> Amounts { get; }
}