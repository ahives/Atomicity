namespace Atomicity;

internal class EmptyOperation :
    IOperation
{
    public Operation CreateOperation(int sequenceNumber) =>
        new()
        {
            SequenceNumber = -1,
            Work = () => false,
            Compensation = () => { }
        };
}