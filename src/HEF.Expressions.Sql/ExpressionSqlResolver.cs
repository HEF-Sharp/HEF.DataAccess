using System.Linq.Expressions;

namespace HEF.Expressions.Sql
{
    public class ExpressionSqlResolver : ExpressionResolver
    {
        protected override bool IsResolveNodeType(Expression expression)
        {
            return expression.IsLambda() ||
                expression.IsMethodCall() ||
                expression.IsMemberAccess() ||
                expression.IsConstant() ||
                expression.IsParameter() ||
                expression.IsLogicOperation() ||
                expression.IsCompareOperation() ||
                expression.IsMathOperation();
        }
    }
}
