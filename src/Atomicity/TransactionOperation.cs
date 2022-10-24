namespace Atomicity;

using Microsoft.Extensions.Logging;
using Configuration;

public abstract class TransactionOperation<TOperation> :
    IOperation
{
    private readonly ILogger<TransactionOperation<TOperation>> _logger;

    protected TransactionOperation(ILogger<TransactionOperation<TOperation>> logger)
    {
        _logger = logger;
    }

    protected TransactionOperation()
    {
    }

    public virtual Operation CreateOperation(int sequenceNumber) =>
        new()
        {
            Name = GetName(),
            SequenceNumber = sequenceNumber,
            Work = DoWork(),
            Compensation = Compensate(),
            Config = Configure()
        };

    protected virtual OperationConfig Configure() => OperationConfigCache.Default;

    protected virtual string GetName() => typeof(TOperation).FullName ?? throw new InvalidOperationException();

    protected virtual Action Compensate() => () =>
    {
        // _logger.LogDebug("You forgot to add compensation logic");
    };

    protected abstract Func<bool> DoWork();
}