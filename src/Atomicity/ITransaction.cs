namespace Atomicity;

using Configuration;

public interface ITransaction
{
    void Execute();

    Transaction AddOperations(IOperation operation, params IOperation[] operations);

    Transaction Configure(Action<TransactionBrokerConfigurator> configurator);

    Transaction Configure();
}