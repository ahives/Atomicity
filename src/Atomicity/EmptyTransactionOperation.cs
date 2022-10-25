namespace Atomicity;

internal class EmptyTransactionOperation :
    IOperation
{
    public TransactionOperation CreateOperation(int sequenceNumber) =>
        new()
        {
            SequenceNumber = -1,
            Work = () => false,
            Compensation = () => { }
        };
}