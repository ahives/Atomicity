namespace Atomicity;

public interface IOperationBuilder
{
    TransactionOperation CreateOperation(Guid transactionId, int sequenceNumber);
}