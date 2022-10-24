namespace Atomicity.Extensions.DependencyInjection;

using Persistence;
using Microsoft.Extensions.DependencyInjection;

public static class AtomicityExtensions
{
    public static IServiceCollection AddAtomicity(this IServiceCollection services)
    {
        services.AddSingleton<IPersistenceProvider, PersistenceProvider>();
        services.AddTransient<ITransactionBroker, Transaction>();

        return services;
    }
}