using HEF.Service;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DbServiceServiceCollectionExtensions
    {
        public static IServiceCollection AddDbService(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddScoped(typeof(IDbService<>), typeof(DbService<>));
            serviceCollection.TryAddScoped(typeof(IDbAsyncService<>), typeof(DbAsyncService<>));

            return serviceCollection;
        }
    }
}
