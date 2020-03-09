using HEF.Expressions.Sql;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ExpressionMySqlServiceCollectionExtensions
    {
        public static IServiceCollection AddExpressionToMySql(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddExpressionToSql();

            serviceCollection.Replace(ServiceDescriptor.Singleton<IMethodCallSqlResolver, MethodCallMySqlResolver>());

            return serviceCollection;
        }
    }
}
