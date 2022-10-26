namespace Atomicity;

using MassTransit;

internal class EmptyOperationBuilder :
    IOperationBuilder
{
    public TransactionOperation CreateOperation(Guid transactionId, int sequenceNumber) =>
        new()
        {
            TransactionId = transactionId,
            OperationId = NewId.NextGuid(),
            SequenceNumber = -1,
            Work = () => false,
            Compensation = () => { }
        };
}