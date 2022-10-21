using Atomicity.Configuration;

namespace Atomicity;

public interface IAtomicity
{
    void Execute(Guid transactionId = default);

    Atomicity AddOperations(Operation operation, params Operation[] operations);

    Atomicity Configure(Action<AtomicityConfigurator> configurator);

    Atomicity Configure();
}