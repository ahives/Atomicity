namespace Atomicity.Persistence;

public interface IDurableTransactionProvider
{
    int GetStartOperation(Guid transactionId);
    
    void Save(Guid transactionId, string operationName, int operationSequenceNumber);
}