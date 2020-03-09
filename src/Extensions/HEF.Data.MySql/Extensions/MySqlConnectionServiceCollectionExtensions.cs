using HEF.Data;
using HEF.Data.MySql;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MySqlConnectionServiceCollectionExtensions
    {
        public static IServiceCollection AddMySqlConnection(this IServiceCollection serviceCollection,
            string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString)); 

            serviceCollection.TryAddScoped<IDbConnectionProvider>(
                (provider) => new MySqlConnectionProvider(new DbOptions().WithConnectionString(connectionString)));

            serviceCollection.AddDbConnectionContext();

            return serviceCollection;
        }
    }
}
