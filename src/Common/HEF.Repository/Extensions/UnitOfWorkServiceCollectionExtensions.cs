using HEF.Repository;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class UnitOfWorkServiceCollectionExtensions
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddScoped<IUnitOfWork, UnitOfWork>();
            serviceCollection.TryAddScoped<IAsyncUnitOfWork, AsyncUnitOfWork>();

            return serviceCollection;
        }
    }
}
