namespace Atomicity.Persistence;

public interface IPersistenceProvider
{
    int GetStartOperation(Guid transactionId);

    bool TrySaveTransaction(Guid transactionId, TransactionState state);

    bool TryUpdateTransaction(Guid transactionId, TransactionState state);
    
    bool TrySaveOperation(Guid transactionId, string operationName, int sequenceNumber, OperationState state);

    bool TryUpdateOperationState(Guid transactionId, OperationState state);

    IReadOnlyList<OperationEntity> GetAllOperations(Guid transactionId);
}