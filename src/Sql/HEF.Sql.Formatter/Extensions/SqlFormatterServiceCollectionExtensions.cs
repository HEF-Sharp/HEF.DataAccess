using HEF.Sql.Formatter;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SqlFormatterServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlFormatter(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ISqlFormatter, SqlFormatter>();

            serviceCollection.AddEntitySqlFormatter();

            return serviceCollection;
        }

        public static IServiceCollection AddSqlServerFormatter(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ISqlFormatter, SqlServerFormatter>();

            serviceCollection.AddEntitySqlFormatter();

            return serviceCollection;
        }

        public static IServiceCollection AddMySqlFormatter(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ISqlFormatter, MySqlFormatter>();

            serviceCollection.AddEntitySqlFormatter();

            return serviceCollection;
        }

        public static IServiceCollection AddOracleFormatter(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ISqlFormatter, OracleFormatter>();

            serviceCollection.AddEntitySqlFormatter();

            return serviceCollection;
        }

        public static IServiceCollection AddPostgreSqlFormatter(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ISqlFormatter, PostgreSqlFormatter>();

            serviceCollection.AddEntitySqlFormatter();

            return serviceCollection;
        }

        public static IServiceCollection AddEntitySqlFormatter(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IEntitySqlFormatter, EntitySqlFormatter>();

            return serviceCollection;
        }
    }
}
