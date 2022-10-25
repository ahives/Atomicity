namespace Atomicity;

internal class EmptyOperationBuilder :
    IOperationBuilder
{
    public TransactionOperation CreateOperation(int sequenceNumber) =>
        new()
        {
            SequenceNumber = -1,
            Work = () => false,
            Compensation = () => { }
        };
}