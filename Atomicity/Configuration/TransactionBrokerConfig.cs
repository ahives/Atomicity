namespace Atomicity.Configuration;

public record TransactionBrokerConfig
{
    public bool ConsoleLoggingOn { get; init; }
    
    public TransactionRetry TransactionRetry { get; init; }
}