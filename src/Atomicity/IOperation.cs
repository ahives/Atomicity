namespace Atomicity;

public interface IOperation
{
    TransactionOperation CreateOperation(int sequenceNumber);
}