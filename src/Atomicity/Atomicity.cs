namespace Atomicity;

using Configuration;

public class Atomicity :
    IAtomicity
{
    private AtomicityConfig _config;
    private readonly ITransactionDurability _transactionDurability;
    private readonly List<Operation> _operations;

    public Atomicity(ITransactionDurability transactionDurability)
    {
        _config = AtomicityConfigCache.Default;
        _transactionDurability = transactionDurability;
        _operations = new List<Operation>();
    }

    public void Execute(Guid transactionId = default)
    {
        int index = -1;
        int start = _transactionDurability.GetStartOperation(transactionId);
        for (int i = start; i < _operations.Count; i++)
        {
            if (_config.ConsoleLoggingOn)
                Console.WriteLine($"Executing operation {i + 1}");
            
            if (_operations[i].Work.Invoke())
            {
                _transactionDurability.Save(transactionId);
                continue;
            }

            index = i;
            break;
        }

        for (int i = index; i >= 0; i--)
        {
            if (_config.ConsoleLoggingOn)
                Console.WriteLine($"Compensating operation {i + 1}");

            _operations[i].Compensation.Invoke();
        }
    }

    public Atomicity AddOperations(Operation operation, params Operation[] operations)
    {
        _operations.Add(operation);

        foreach (var op in operations)
        {
            _operations.Add(op);
        }
        
        return this;
    }

    public Atomicity Configure(Action<AtomicityConfigurator> configurator)
    {
        AtomicityConfiguratorImpl impl = new AtomicityConfiguratorImpl();
        configurator?.Invoke(impl);

        var config = new AtomicityConfig
        {
            ConsoleLoggingOn = impl.ConsoleLoggingOn,
            TransactionRetry = impl.TransactionRetry
        };

        _config = config;

        return this;
    }

    public Atomicity Configure()
    {
        return this;
    }

    
    class AtomicityConfiguratorImpl :
        AtomicityConfigurator
    {
        public bool LoggingOn { get; private set; }
        public bool ConsoleLoggingOn { get; private set; }
        public TransactionRetry TransactionRetry { get; private set; }


        public AtomicityConfiguratorImpl()
        {
            TransactionRetry = TransactionRetry.None;
        }

        public void TurnOnLogging() => LoggingOn = true;

        public void TurnOnConsoleLogging() => ConsoleLoggingOn = true;
        
        public void Retry(TransactionRetry retry = TransactionRetry.None) => TransactionRetry = retry;
    }
}