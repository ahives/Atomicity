namespace Atomicity;

public enum TransactionState
{
    New = 1,
    Pending = 2,
    Faulted = 3,
    Completed = 4,
    Compensated = 5
}