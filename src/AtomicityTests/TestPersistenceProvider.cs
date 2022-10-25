namespace AtomicityTests;

using Atomicity.Persistence;
using MassTransit;

public class TestPersistenceProvider :
    IPersistenceProvider
{
    public int GetStartOperation(Guid transactionId)
    {
        return 0;
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
        var operations = new List<OperationEntity>();

        operations.Add(new OperationEntity()
        {
            Id = NewId.NextGuid(), Name = "test-1", TransactionId = transactionId, SequenceNumber = 1, State = 1,
            CreationTimestamp = DateTimeOffset.UtcNow
        });
        operations.Add(new OperationEntity()
        {
            Id = NewId.NextGuid(), Name = "test-2", TransactionId = transactionId, SequenceNumber = 2, State = 1,
            CreationTimestamp = DateTimeOffset.UtcNow
        });
        operations.Add(new OperationEntity()
        {
            Id = NewId.NextGuid(), Name = "test-3", TransactionId = transactionId, SequenceNumber = 3, State = 1,
            CreationTimestamp = DateTimeOffset.UtcNow
        });

        return operations;
    }
}