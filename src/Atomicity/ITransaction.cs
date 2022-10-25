namespace Atomicity;

using Configuration;

public interface ITransaction
{
    void Execute();

    Transaction AddOperations(IOperationBuilder builder, params IOperationBuilder[] builders);

    Transaction Configure(Action<TransactionBrokerConfigurator> configurator);

    Transaction Configure();
}