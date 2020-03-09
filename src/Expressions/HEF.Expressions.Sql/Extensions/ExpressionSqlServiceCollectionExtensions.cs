using HEF.Expressions.Sql;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ExpressionSqlServiceCollectionExtensions
    {
        public static IServiceCollection AddExpressionToSql(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IMethodCallSqlResolver, MethodCallSqlResolver>();

            serviceCollection.TryAddSingleton<IExpressionSqlResolver, ExpressionSqlResolver>();

            return serviceCollection;
        }
    }
}
