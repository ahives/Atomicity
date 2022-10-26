namespace Atomicity.Persistence;

public interface IPersistenceProvider
{
    int GetStartOperation(Guid transactionId);

    bool TrySaveTransaction(Guid transactionId, TransactionState state);

    bool TryUpdateTransaction(Guid transactionId, TransactionState state);
    
    bool TrySaveOperation(TransactionOperation operation, OperationState state);

    bool TryUpdateOperationState(Guid operationId, OperationState state);

    IReadOnlyList<OperationEntity> GetAllOperations(Guid transactionId);
}