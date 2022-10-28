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
        ThrowIfSaveFailed(_persistence.TrySaveTransaction, _transactionId);

        var op = builder.Create(_transactionId, _operations.Count + 1);
        _operations.Add(op);

        ThrowIfSaveFailed(_persistence.TrySaveOperation, op);

        ThrowIfUpdateFailed(_persistence.TryUpdateTransaction, _transactionId, TransactionState.Pending);

        for (int i = 0; i < builders.Length; i++)
        {
            var operation = builders[i].Create(_transactionId, _operations.Count + 1);
            _operations.Add(operation);
            
            ThrowIfSaveFailed(_persistence.TrySaveOperation, operation);
        }

        return this;
    }

    public void Execute()
    {
        // if (!IsVerified(out ValidationResult report))
        //     return;

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

    bool IsVerified(out ValidationResult report)
    {
        if (!_persistence.TryGetTransaction(_transactionId, out TransactionEntity transaction))
        {
            report = new() {TransactionId = _transactionId, Message = "Could not find transaction in the database."};
            return false;
        }
        
        var operations = _persistence.GetAllOperations(_transactionId);

        if (operations.Count != _operations.Count)
        {
            report = new(){TransactionId = _transactionId, Message = ""};
            return true;
        }

        List<ValidationResult> results = new List<ValidationResult>();
        for (int i = 0; i < operations.Count; i++)
        {
            if (_operations.All(x => x.OperationId != operations[i].Id))
            {
                results.Add(new()
                {
                    TransactionId = _transactionId,
                    OperationId = operations[i].Id,
                    Disposition = Disposition.Missing,
                    Message = ""
                });
                continue;
            }
        }
        
        // TODO: add logic to compare database and in-memory operations before executing
        report = new ValidationResult();
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

        ThrowIfUpdateFailed(_persistence.TryUpdateTransaction, _transactionId, TransactionState.Pending);
        
        var operations = _persistence.GetAllOperations(_transactionId);
        var results = new List<ValidationResult>();

        for (int i = start; i < _operations.Count; i++)
        {
            if (_config.ConsoleLoggingOn)
                Console.WriteLine($"Executing operation {_operations[i].SequenceNumber}");

            if (!_operations[i].VerifyIsExecutable(_transactionId, operations, out ValidationResult result))
            {
                results.Add(result);
                continue;
            }
            
            // TODO: if the current state is completed skip to the next operation
            
            ThrowIfUpdateFailed(_persistence.TryUpdateOperationState, transactionId, OperationState.Pending);

            if (_operations[i].Work.Invoke())
            {
                ThrowIfUpdateFailed(_persistence.TryUpdateOperationState, transactionId, OperationState.Completed);
                continue;
            }

            ThrowIfUpdateFailed(_persistence.TryUpdateOperationState, transactionId, OperationState.Failed);

            operationFailed = true;
            faultedIndex = i;
            break;
        }

        if (operationFailed)
            ThrowIfUpdateFailed(_persistence.TryUpdateTransaction, _transactionId, TransactionState.Failed);

        return operationFailed;
    }

    bool TryDoCompensation(Guid transactionId, int faultedIndex)
    {
        if (!_persistence.TryUpdateTransaction(transactionId, TransactionState.Failed))
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

    void ThrowIfSaveFailed(Func<Guid, bool> save, Guid transactionId)
    {
        if (save is null)
            throw new ArgumentNullException();
        
        if (save.Invoke(transactionId))
            return;

        throw new TransactionPersistenceException();
    }

    void ThrowIfSaveFailed(Func<TransactionOperation, bool> save, TransactionOperation operation)
    {
        if (save is null)
            throw new ArgumentNullException();
        
        if (save.Invoke(operation))
            return;

        throw new TransactionPersistenceException();
    }

    void ThrowIfUpdateFailed(Func<Guid, TransactionState, bool> update, Guid transactionId, TransactionState state)
    {
        if (update is null)
            throw new ArgumentNullException();
        
        if (update.Invoke(transactionId, state))
            return;

        throw new TransactionPersistenceException();
    }

    void ThrowIfUpdateFailed(Func<Guid, OperationState, bool> update, Guid transactionId, OperationState state)
    {
        if (update is null)
            throw new ArgumentNullException();
        
        if (update.Invoke(transactionId, state))
            return;

        throw new TransactionPersistenceException();
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