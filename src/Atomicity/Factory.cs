namespace Atomicity;

public static class Factory
{
    public static IOperation CreateOperation<T>()
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