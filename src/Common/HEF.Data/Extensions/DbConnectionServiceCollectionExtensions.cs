using HEF.Data;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DbConnectionServiceCollectionExtensions
    {
        public static IServiceCollection AddDbConnectionContext(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddScoped<IDbConnectionContext, DbConnectionContext>();
            serviceCollection.TryAddScoped<IDbAsyncConnectionContext, DbAsyncConnectionContext>();

            return serviceCollection;
        }
    }
}
