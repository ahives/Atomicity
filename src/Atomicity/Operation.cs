namespace Atomicity;

using Microsoft.Extensions.Logging;
using Configuration;

public abstract class Operation<TOperation> :
    IOperation
{
    private readonly ILogger<Operation<TOperation>> _logger;

    protected Operation(ILogger<Operation<TOperation>> logger)
    {
        _logger = logger;
    }

    protected Operation()
    {
    }

    public virtual TransactionOperation CreateOperation(int sequenceNumber) =>
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

public static class Operation
{
    public static IOperation Create<T>()
        where T : class, IOperation
    {
        if (typeof(T).IsAssignableTo(typeof(IOperation)))
            return CreateInstance(typeof(T));

        return OperationsCache.Empty;
    }

    static IOperation CreateInstance(Type type)
    {
        try
        {
            return Activator.CreateInstance(type) as IOperation ?? OperationsCache.Empty;
        }
        catch (Exception e)
        {
            return OperationsCache.Empty;
        }
    }
}