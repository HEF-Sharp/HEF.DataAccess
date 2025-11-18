using HEF.Expressions.Sql;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ExpressionOracleServiceCollectionExtensions
    {
        public static IServiceCollection AddExpressionToOracle(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddExpressionToSql();

            serviceCollection.AddMethodCallExpressionOracleResolver();

            return serviceCollection;
        }

        private static IServiceCollection AddMethodCallExpressionOracleResolver(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, DateTimeAddMethodsOracleResolveExecutor>();

            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, DecimalRoundMethodsOracleResolveExecutor>();

            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, MathRoundMethodsOracleResolveExecutor>();

            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, StringStartsWithOracleResolveExecutor>();
            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, StringEndsWithOracleResolveExecutor>();
            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, StringContainsOracleResolveExecutor>();
            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, StringConcatOracleResolveExecutor>();
            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, StringSubstringOracleResolveExecutor>();

            return serviceCollection;
        }
    }
}
