using HEF.Data;
using HEF.Data.Query;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DbEntityQueryServiceCollectionExtensions
    {
        public static IServiceCollection AddEntityQueryable(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IQueryableExpressionVisitorFactory, QueryableExpressionVisitorFactory>();
            serviceCollection.TryAddScoped<IDbCommandBuilderFactory, DbCommandBuilderFactory>();
            serviceCollection.TryAddScoped<IConcurrencyDetector, ConcurrencyDetector>();

            serviceCollection.TryAddScoped<IDbEntityQueryExecutor, DbEntityQueryExecutor>();
            serviceCollection.TryAddScoped<IAsyncQueryProvider, DbEntityQueryProvider>();

            return serviceCollection;
        }
    }
}
