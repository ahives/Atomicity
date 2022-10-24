namespace Atomicity;

using Configuration;
using Persistence;

public class TransactionBroker :
    ITransactionBroker
{
    private TransactionBrokerConfig _config;
    private readonly ITransactionPersistenceProvider _persistenceProvider;
    private readonly List<Operation> _operations;

    public TransactionBroker(ITransactionPersistenceProvider persistenceProvider)
    {
        _config = AtomicityConfigCache.Default;
        _persistenceProvider = persistenceProvider;
        _operations = new List<Operation>();
    }

    public TransactionBroker Configure(Action<TransactionBrokerConfigurator> configurator)
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

    public TransactionBroker Configure()
    {
        return this;
    }

    public TransactionBroker AddOperations(IOperation operation, params IOperation[] operations)
    {
        _operations.Add(operation.CreateOperation(_operations.Count + 1));

        foreach (var op in operations)
        {
            _operations.Add(op.CreateOperation(_operations.Count + 1));
        }
        
        return this;
    }

    public void Execute(Guid transactionId = default)
    {
        int index = -1;
        int start = _persistenceProvider.GetStartOperation(transactionId);
        for (int i = start; i < _operations.Count; i++)
        {
            if (_config.ConsoleLoggingOn)
                Console.WriteLine($"Executing operation {_operations[i].SequenceNumber}");
            
            if (_operations[i].Work.Invoke())
            {
                _persistenceProvider.Save(transactionId, _operations[i].Name, _operations[i].SequenceNumber);
                continue;
            }

            index = i;
            break;
        }

        for (int i = index; i >= 0; i--)
        {
            if (_config.ConsoleLoggingOn)
                Console.WriteLine($"Compensating operation {_operations[i].SequenceNumber}");

            _operations[i].Compensation.Invoke();
        }
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