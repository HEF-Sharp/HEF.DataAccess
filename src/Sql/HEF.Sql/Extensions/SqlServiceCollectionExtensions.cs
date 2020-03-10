using HEF.Sql;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SqlServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlBuilder(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<ISqlBuilderFactory, SqlBuilderFactory>();

            return serviceCollection;
        }
    }
}
