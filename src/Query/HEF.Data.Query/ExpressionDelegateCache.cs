using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace HEF.Data.Query
{
    internal static class ExpressionDelegateCache
    {
        private static readonly ConcurrentDictionary<string, Delegate> _expressionDelegateCache
            = new ConcurrentDictionary<string, Delegate>();

        internal static Delegate GetExpressionDelegate(string expressionKey,
            Func<string, LambdaExpression> expressionFactory)
        {
            return _expressionDelegateCache.GetOrAdd(expressionKey, key =>
            {
                var lambdaExpr = expressionFactory.Invoke(key);
                return lambdaExpr.Compile();
            });
        }
    }
}
