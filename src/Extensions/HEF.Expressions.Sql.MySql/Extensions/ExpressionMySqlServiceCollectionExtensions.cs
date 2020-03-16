using HEF.Expressions.Sql;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ExpressionMySqlServiceCollectionExtensions
    {
        public static IServiceCollection AddExpressionToMySql(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddExpressionToSql();

            serviceCollection.AddMethodCallExpressionMySqlResolver();

            return serviceCollection;
        }

        private static IServiceCollection AddMethodCallExpressionMySqlResolver(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, DateTimeAddMethodsMySqlResolveExecutor>();

            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, DecimalRoundMethodsMySqlResolveExecutor>();

            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, MathRoundMethodsMySqlResolveExecutor>();

            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, StringStartsWithMySqlResolveExecutor>();
            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, StringEndsWithMySqlResolveExecutor>();
            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, StringContainsMySqlResolveExecutor>();
            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, StringConcatMySqlResolveExecutor>();
            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, StringSubstringMySqlResolveExecutor>();

            return serviceCollection;
        }
    }
}
