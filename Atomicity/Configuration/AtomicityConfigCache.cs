namespace Atomicity.Configuration;

public static class AtomicityConfigCache
{
    public static readonly TransactionBrokerConfig Default = new()
    {
        ConsoleLoggingOn = true
    };
}