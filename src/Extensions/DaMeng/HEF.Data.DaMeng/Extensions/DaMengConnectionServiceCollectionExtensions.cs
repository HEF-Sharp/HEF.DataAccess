using HEF.Data;
using HEF.Data.DaMeng;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DaMengConnectionServiceCollectionExtensions
    {
        public static IServiceCollection AddDaMengConnection(this IServiceCollection serviceCollection,
            string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            serviceCollection.TryAddScoped<IDbConnectionProvider>(
                (provider) => new DaMengConnectionProvider(new DbOptions().WithConnectionString(connectionString)));

            serviceCollection.AddDbConnectionContext();

            return serviceCollection;
        }
    }
}
