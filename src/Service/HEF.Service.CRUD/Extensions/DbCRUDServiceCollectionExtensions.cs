using HEF.Service.CRUD;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DbCRUDServiceCollectionExtensions
    {
        public static IServiceCollection AddDbCRUDService(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddScoped(typeof(IDbCRUDService<>), typeof(DbCRUDService<>));
            serviceCollection.TryAddScoped(typeof(IDbAsyncCRUDService<>), typeof(DbAsyncCRUDService<>));

            return serviceCollection;
        }
    }
}
