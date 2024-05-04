using Models.Results;

namespace Interfaces.Repositories;

public interface ITransactionRepository
{
    Task CreateOperation(long userId, long accountId, long amount);
    Task<TransactionResult> GetOperation(long accountId);
}