namespace Atomicity.Persistence;

public enum TransactionState
{
    New,
    Pending,
    Completed,
    Faulted
}