namespace Atomicity.Configuration;

public record AtomicityConfig
{
    public bool ConsoleLoggingOn { get; init; }
    
    public TransactionRetry TransactionRetry { get; init; }
}