namespace Atomicity.Persistence;

public interface IPersistenceProvider
{
    int GetStartOperation(Guid transactionId);

    bool SaveTransaction(Guid transactionId, TransactionState state);

    bool UpdateTransaction(Guid transactionId, TransactionState state);
    
    bool TrySaveOperation(Guid transactionId, string operationName, int sequenceNumber);

    bool TryUpdateOperationState(Guid transactionId, OperationState state);

    IReadOnlyList<OperationEntity> GetAllOperations(Guid transactionId);
}