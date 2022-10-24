namespace Atomicity;

using Configuration;

public interface ITransactionBroker
{
    void Execute(Guid transactionId = default);

    Transaction AddOperations(IOperation operation, params IOperation[] operations);

    Transaction Configure(Action<TransactionBrokerConfigurator> configurator);

    Transaction Configure();
}