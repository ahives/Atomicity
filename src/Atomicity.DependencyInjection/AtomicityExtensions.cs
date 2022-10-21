namespace Atomicity.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

public static class AtomicityExtensions
{
    public static IServiceCollection AddAtomicity(this IServiceCollection services)
    {
        services.AddSingleton<ITransactionDurability, TransactionDurability>();
        services.AddSingleton<IAtomicity, Atomicity>();

        return services;
    }
}