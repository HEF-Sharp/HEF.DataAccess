using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace HEF.Data.Query
{
    internal static class LambdaExpressionCache
    {
        private static readonly ConcurrentDictionary<string, LambdaExpression> _lambdaExpressionCache
            = new ConcurrentDictionary<string, LambdaExpression>();

        internal static LambdaExpression GetLambdaExpression(string expressionKey,
            Func<string, LambdaExpression> expressionFactory)
        {
            return _lambdaExpressionCache.GetOrAdd(expressionKey, expressionFactory);
        }
    }
}
