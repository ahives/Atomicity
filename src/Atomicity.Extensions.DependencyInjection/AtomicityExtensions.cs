using Atomicity.Persistence;

namespace Atomicity.Extensions.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

public static class AtomicityExtensions
{
    public static IServiceCollection AddAtomicity(this IServiceCollection services)
    {
        services.AddSingleton<IDurableTransactionProvider, DurableTransactionProvider>();
        services.AddSingleton<IAtomicity, Atomicity>();

        return services;
    }
}