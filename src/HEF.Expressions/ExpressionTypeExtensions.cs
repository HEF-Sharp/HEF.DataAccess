using System.Linq.Expressions;

namespace HEF.Expressions
{
    public static class ExpressionTypeExtensions
    {
        public static bool IsLambda(this Expression expr)
        {
            return expr.NodeType == ExpressionType.Lambda;
        }

        public static bool IsMethodCall(this Expression expr)
        {
            return expr.NodeType == ExpressionType.Call;
        }

        public static bool IsMemberAccess(this Expression expr)
        {
            return expr.NodeType == ExpressionType.MemberAccess;
        }

        public static bool IsConstant(this Expression expr)
        {
            return expr.NodeType == ExpressionType.Constant;
        }

        public static bool IsParameter(this Expression expr)
        {
            return expr.NodeType == ExpressionType.Parameter;
        }

        public static bool IsLogicOperation(this Expression expr)
        {
            return expr.NodeType == ExpressionType.And ||
                expr.NodeType == ExpressionType.AndAlso ||
                expr.NodeType == ExpressionType.Or ||
                expr.NodeType == ExpressionType.OrElse;
        }

        public static bool IsCompareOperation(this Expression expr)
        {
            return expr.NodeType == ExpressionType.LessThan ||
                expr.NodeType == ExpressionType.LessThanOrEqual ||
                expr.NodeType == ExpressionType.GreaterThan ||
                expr.NodeType == ExpressionType.GreaterThanOrEqual ||
                expr.NodeType == ExpressionType.Equal ||
                expr.NodeType == ExpressionType.NotEqual;
        }

        public static bool IsMathOperation(this Expression expr)
        {
            return expr.NodeType == ExpressionType.Add ||
                expr.NodeType == ExpressionType.AddChecked ||
                expr.NodeType == ExpressionType.Subtract ||
                expr.NodeType == ExpressionType.SubtractChecked ||
                expr.NodeType == ExpressionType.Multiply ||
                expr.NodeType == ExpressionType.MultiplyChecked ||
                expr.NodeType == ExpressionType.Divide ||
                expr.NodeType == ExpressionType.Modulo;
        }
    }
}
