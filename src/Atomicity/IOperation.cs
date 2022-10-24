namespace Atomicity;

public interface IOperation
{
    Operation CreateOperation(int sequenceNumber);
}