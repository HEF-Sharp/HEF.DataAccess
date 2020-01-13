using System.Linq.Expressions;

namespace HEF.Data.Query
{
    internal static class ExpressionExtensions
    {
        internal static bool IsDbEntityQueryable(this ConstantExpression constantExpression)
            => constantExpression.Type.IsGenericType
                && constantExpression.Type.GetGenericTypeDefinition() == typeof(DbEntityQueryable<>);

        internal static LambdaExpression UnwrapLambdaFromQuote(this Expression expression)
            => (LambdaExpression)
            (expression is UnaryExpression unary && expression.NodeType == ExpressionType.Quote
                ? unary.Operand
                : expression);
    }
}
