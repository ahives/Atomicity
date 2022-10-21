namespace Atomicity;

public static class Operations
{
    public static Operation Create<T>()
        where T : class, IOperation
    {
        if (typeof(T).IsAssignableTo(typeof(IOperation)))
            return CreateInstance(typeof(T)).Create();

        return null;
    }

    static IOperation CreateInstance(Type type) => Activator.CreateInstance(type) as IOperation;
}