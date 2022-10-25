namespace Atomicity.Persistence;

public class PersistenceProvider :
    IPersistenceProvider
{
    public int GetStartOperation(Guid transactionId)
    {
        using var db = new TransactionDbContext();

        var operation = (from op in db.Operations
                where op.Id == transactionId && op.State != 1
                orderby op.SequenceNumber
                select op)
            .FirstOrDefault();

        if (operation is null)
            return 0;

        return operation.SequenceNumber - 1;
    }

    public bool SaveTransaction(Guid transactionId)
    {
        return true;
    }

    public bool UpdateTransaction(Guid transactionId, TransactionState state)
    {
        return true;
    }

    public bool TrySaveOperation(Guid transactionId, string operationName, int sequenceNumber)
    {
        return true;
    }

    public bool TryUpdateOperationState(Guid transactionId, OperationState state)
    {
        return true;
    }

    public IReadOnlyList<OperationEntity> GetAllOperations(Guid transactionId)
    {
        throw new NotImplementedException();
    }
}