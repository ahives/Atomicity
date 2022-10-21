namespace Atomicity.Configuration;

public static class AtomicityConfigCache
{
    public static readonly AtomicityConfig Default = new()
    {
        ConsoleLoggingOn = true
    };
}