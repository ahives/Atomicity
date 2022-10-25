namespace Atomicity.Persistence;

public enum OperationState
{
    New,
    Pending,
    Completed,
    Faulted,
    Compensated
}