namespace Atomicity;

using Configuration;

public abstract class AtomicityOperation :
    IOperation
{
    public virtual Operation Create()
    {
        return new Operation
        {
            Work = Work(),
            Compensation = Compensation(),
            Config = Configure()
        };
    }

    protected virtual OperationConfig Configure() => OperationConfigCache.Default;

    protected virtual Action Compensation() => () => { };

    protected abstract Func<bool> Work();
}