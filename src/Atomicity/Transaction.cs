using System.Buffers;

namespace Atomicity;

using Configuration;
using Persistence;
using MassTransit;

public class Transaction :
    ITransaction
{
    private TransactionBrokerConfig _config;
    private readonly IPersistenceProvider _persistence;
    private readonly List<TransactionOperation> _operations;
    private readonly Guid _transactionId;

    public Transaction(IPersistenceProvider persistence)
    {
        _config = AtomicityConfigCache.Default;
        _persistence = persistence;
        _operations = new List<TransactionOperation>();
        _transactionId = NewId.NextGuid();
    }

    public Transaction Configure(Action<TransactionBrokerConfigurator> configurator)
    {
        TransactionBrokerConfiguratorImpl impl = new TransactionBrokerConfiguratorImpl();
        configurator?.Invoke(impl);

        var config = new TransactionBrokerConfig
        {
            ConsoleLoggingOn = impl.ConsoleLoggingOn,
            TransactionRetry = impl.TransactionRetry
        };

        _config = config;

        return this;
    }

    public Transaction Configure()
    {
        return this;
    }

    public Transaction AddOperations(IOperationBuilder builder, params IOperationBuilder[] builders)
    {
        if (!_persistence.TrySaveTransaction(_transactionId))
            throw new TransactionPersistenceException();

        var op = builder.CreateOperation(_transactionId, _operations.Count + 1);
        _operations.Add(op);

        if (!_persistence.TrySaveOperation(op))
            throw new TransactionPersistenceException();

        if (!_persistence.TryUpdateTransaction(_transactionId, TransactionState.Pending))
            throw new TransactionPersistenceException();

        for (int i = 0; i < builders.Length; i++)
        {
            var operation = builders[i].CreateOperation(_transactionId, _operations.Count + 1);
            _operations.Add(operation);
            
            if (!_persistence.TrySaveOperation(operation))
                throw new TransactionPersistenceException();
        }

        return this;
    }

    public void Execute()
    {
        if (!IsVerified(out TransactionReport report))
            return;

        if (!TryDoWork(_transactionId, out int faultedIndex))
            return;

        bool compensated = TryDoCompensation(_transactionId, faultedIndex);
    }

    public static Transaction Create()
    {
        return new Transaction(new PersistenceProvider());
    }

    public static Transaction Create(IPersistenceProvider provider)
    {
        return new Transaction(provider);
    }

    bool IsVerified(out TransactionReport report)
    {
        // TODO: add logic to compare database and in-memory operations before executing
        report = new TransactionReport();
        return true;
    }

    // public IReadOnlyList<OperationStatus> GetOperationStatus()
    // {
    //     return _persistence
    //         .GetAllOperations(_transactionId)
    //         .Select(x => new {x.TransactionId, OperationId = x.Id, (OperationState)x.State});
    // }

    bool TryDoWork(Guid transactionId, out int faultedIndex)
    {
        bool operationFailed = false;
        int start = _persistence.GetStartOperation(transactionId);

        faultedIndex = -1;

        if (!_persistence.TryUpdateTransaction(_transactionId, TransactionState.Pending))
            throw new TransactionPersistenceException();

        for (int i = start; i < _operations.Count; i++)
        {
            if (_config.ConsoleLoggingOn)
                Console.WriteLine($"Executing operation {_operations[i].SequenceNumber}");

            if (!_persistence.TryUpdateOperationState(_operations[i].OperationId, OperationState.Pending))
                throw new TransactionPersistenceException();

            if (_operations[i].Work.Invoke())
            {
                if (!_persistence.TryUpdateOperationState(transactionId, OperationState.Completed))
                    throw new TransactionPersistenceException();

                continue;
            }

            _persistence.TryUpdateOperationState(transactionId, OperationState.Faulted);

            operationFailed = true;
            faultedIndex = i;
            break;
        }

        if (operationFailed)
        {
            if (!_persistence.TryUpdateTransaction(_transactionId, TransactionState.Faulted))
                throw new TransactionPersistenceException();
        }
        else
        {
            if (!_persistence.TryUpdateTransaction(_transactionId, TransactionState.Completed))
                throw new TransactionPersistenceException();
        }

        return operationFailed;
    }

    bool TryDoCompensation(Guid transactionId, int faultedIndex)
    {
        if (!_persistence.TryUpdateTransaction(transactionId, TransactionState.Faulted))
            return false;

        for (int i = faultedIndex; i >= 0; i--)
        {
            if (_config.ConsoleLoggingOn)
                Console.WriteLine($"Compensating operation {_operations[i].SequenceNumber}");

            _operations[i].Compensation.Invoke();

            if (!_persistence.TryUpdateOperationState(_operations[i].OperationId, OperationState.Compensated))
                throw new TransactionPersistenceException();
        }

        if (!_persistence.TryUpdateTransaction(_transactionId, TransactionState.Compensated))
            throw new TransactionPersistenceException();

        return true;
    }


    class TransactionBrokerConfiguratorImpl :
        TransactionBrokerConfigurator
    {
        public bool LoggingOn { get; private set; }
        public bool ConsoleLoggingOn { get; private set; }
        public TransactionRetry TransactionRetry { get; private set; }


        public TransactionBrokerConfiguratorImpl()
        {
            TransactionRetry = TransactionRetry.None;
        }

        public void TurnOnLogging() => LoggingOn = true;

        public void TurnOnConsoleLogging() => ConsoleLoggingOn = true;
        
        public void Retry(TransactionRetry retry = TransactionRetry.None) => TransactionRetry = retry;
    }
}