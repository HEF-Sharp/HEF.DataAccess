using HEF.Expressions.Sql;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ExpressionSqlServiceCollectionExtensions
    {
        public static IServiceCollection AddExpressionToSql(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddMethodCallExpressionSqlResolver();

            serviceCollection.TryAddSingleton<IExpressionSqlResolver, ExpressionSqlResolver>();

            return serviceCollection;
        }

        private static IServiceCollection AddMethodCallExpressionSqlResolver(this IServiceCollection serviceCollection)
        {
            #region ResolveExecutors
            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, ObjectEqualsSqlResolveExecutor>();
            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, EquatableSqlResolveExecutor>();

            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, CompareSqlResolveExecutor>();
            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, ComparableSqlResolveExecutor>();

            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, DateTimeAddMethodsSqlResolveExecutor>();

            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, DecimalMathOperationsSqlResolveExecutor>();
            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, DecimalRoundMethodsSqlResolveExecutor>();

            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, MathRoundMethodsSqlResolveExecutor>();

            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, StringStartsWithSqlResolveExecutor>();
            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, StringEndsWithSqlResolveExecutor>();
            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, StringContainsSqlResolveExecutor>();
            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, StringConcatSqlResolveExecutor>();
            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, StringIsNullOrEmptySqlResolveExecutor>();
            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, StringToUpperSqlResolveExecutor>();
            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, StringToLowerSqlResolveExecutor>();
            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, StringReplaceSqlResolveExecutor>();
            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, StringSubstringSqlResolveExecutor>();
            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, StringTrimSqlResolveExecutor>();

            serviceCollection.AddSingleton<IMethodCallSqlResolveExecutor, EnumerableContainsMethodsSqlResolveExecutor>();
            #endregion

            serviceCollection.TryAddSingleton<IMethodCallSqlResolver, MethodCallSqlResolver>();

            return serviceCollection;
        }
    }
}
