namespace Atomicity;

public interface IOperationBuilder
{
    TransactionOperation Create(Guid transactionId, int sequenceNumber);
}