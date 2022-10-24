namespace Atomicity.Extensions.DependencyInjection;

using Persistence;
using Microsoft.Extensions.DependencyInjection;

public static class AtomicityExtensions
{
    public static IServiceCollection AddAtomicity(this IServiceCollection services)
    {
        services.AddSingleton<ITransactionPersistenceProvider, TransactionPersistenceProvider>();
        services.AddTransient<ITransactionBroker, TransactionBroker>();

        return services;
    }
}