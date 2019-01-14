using System.Linq.Expressions;

namespace HEF.Data.Query.ExpressionVisitors
{
    public interface IEvaluatableExpressionFilter
    {
        bool IsEvaluatableExpression(Expression expression);
    }
}
