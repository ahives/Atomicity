namespace Atomicity.Configuration;

public interface TransactionBrokerConfigurator
{
    void TurnOnLogging();
    
    void TurnOnConsoleLogging();

    void Retry(TransactionRetry retry = TransactionRetry.None);
}