namespace Atomicity;

public interface IOperationBuilder
{
    TransactionOperation CreateOperation(int sequenceNumber);
}