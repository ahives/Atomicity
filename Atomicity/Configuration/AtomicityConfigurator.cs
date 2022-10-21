namespace Atomicity.Configuration;

public interface AtomicityConfigurator
{
    void TurnOnLogging();
    
    void TurnOnConsoleLogging();

    void Retry(TransactionRetry retry = TransactionRetry.None);
}