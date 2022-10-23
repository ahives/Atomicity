namespace Atomicity;

using Microsoft.Extensions.Logging;
using Configuration;

public abstract class AtomicityOperation<TOperation> :
    IOperation
{
    private readonly ILogger<AtomicityOperation<TOperation>> _logger;

    protected AtomicityOperation(ILogger<AtomicityOperation<TOperation>> logger)
    {
        _logger = logger;
    }

    protected AtomicityOperation()
    {
    }

    public virtual Operation CreateOperation() =>
        new()
        {
            Name = GetName(),
            Work = DoWork(),
            Compensation = Compensate(),
            Config = Configure()
        };

    protected virtual OperationConfig Configure() => OperationConfigCache.Default;

    protected virtual string GetName() => typeof(TOperation).FullName ?? throw new InvalidOperationException();

    protected virtual Action Compensate() => () =>
    {
        _logger.LogDebug("");
    };

    protected abstract Func<bool> DoWork();
}