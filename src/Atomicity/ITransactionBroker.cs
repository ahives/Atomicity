namespace Atomicity;

using Configuration;

public interface ITransactionBroker
{
    void Execute(Guid transactionId = default);

    TransactionBroker AddOperations(IOperation operation, params IOperation[] operations);

    TransactionBroker Configure(Action<TransactionBrokerConfigurator> configurator);

    TransactionBroker Configure();
}