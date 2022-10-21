namespace Atomicity.Configuration;

public record OperationConfig
{
    public TransactionRetry TransactionRetry { get; init; }
}