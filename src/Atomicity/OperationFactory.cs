namespace Atomicity;

public static class OperationFactory
{
    public static Operation Create<T>()
        where T : class, IOperation
    {
        if (typeof(T).IsAssignableTo(typeof(IOperation)))
            return CreateInstance(typeof(T)).CreateOperation();

        return null;
    }

    static IOperation CreateInstance(Type type) => Activator.CreateInstance(type) as IOperation;
}