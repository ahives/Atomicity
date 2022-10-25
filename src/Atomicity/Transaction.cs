namespace Atomicity;

using Configuration;
using Persistence;
using MassTransit;

public class Transaction :
    ITransaction
{
    private TransactionBrokerConfig _config;
    private readonly IPersistenceProvider _persistenceProvider;
    private readonly List<TransactionOperation> _operations;
    private Guid _transactionId;

    public Transaction(IPersistenceProvider persistenceProvider)
    {
        _config = AtomicityConfigCache.Default;
        _persistenceProvider = persistenceProvider;
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

    public Transaction AddOperations(IOperation operation, params IOperation[] operations)
    {
        if (!_persistenceProvider.SaveTransaction(_transactionId, TransactionState.New))
            return this;

        var op = operation.CreateOperation(_operations.Count + 1);
        _operations.Add(op);

        _persistenceProvider.TrySaveOperation(_transactionId, op.Name, op.SequenceNumber);

        for (int i = 0; i < operations.Length; i++)
        {
            _operations.Add(operations[i].CreateOperation(_operations.Count + 1));
            
            // TODO: add retry logic here later to ensure operations are saved before continue
            _persistenceProvider.TrySaveOperation(_transactionId, _operations[i].Name,
                _operations[i].SequenceNumber);
        }
        
        _persistenceProvider.SaveTransaction(_transactionId, TransactionState.Pending);

        return this;
    }

    public void Execute()
    {
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

    bool TryDoWork(Guid transactionId, out int faultedIndex)
    {
        bool operationFailed = false;
        int start = _persistenceProvider.GetStartOperation(transactionId);

        faultedIndex = -1;
        
        for (int i = start; i < _operations.Count; i++)
        {
            if (_config.ConsoleLoggingOn)
                Console.WriteLine($"Executing operation {_operations[i].SequenceNumber}");

            if (_operations[i].Work.Invoke())
            {
                _persistenceProvider.TryUpdateOperationState(transactionId, OperationState.Completed);
                continue;
            }

            _persistenceProvider.TryUpdateOperationState(transactionId, OperationState.Faulted);

            operationFailed = true;
            faultedIndex = i;
            break;
        }

        return operationFailed;
    }

    bool TryDoCompensation(Guid transactionId, int faultedIndex)
    {
        bool transactionUpdated = _persistenceProvider.UpdateTransaction(transactionId, TransactionState.Faulted);

        if (!transactionUpdated)
            return false;

        var operations = _persistenceProvider.GetAllOperations(transactionId);

        for (int i = faultedIndex; i >= 0; i--)
        {
            if (_config.ConsoleLoggingOn)
                Console.WriteLine($"Compensating operation {_operations[i].SequenceNumber}");

            _operations[i].Compensation.Invoke();

            _persistenceProvider.TryUpdateOperationState(operations[i].Id, OperationState.Compensated);
        }

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