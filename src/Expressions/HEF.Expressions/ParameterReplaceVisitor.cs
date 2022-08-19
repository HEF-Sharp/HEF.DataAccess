using System;
using System.Linq.Expressions;

namespace HEF.Expressions
{
    /// <summary>
    /// 参数替换 ExpressionVisitor
    /// </summary>
    public class ParameterReplaceVisitor : ExpressionVisitor
    {
        public ParameterReplaceVisitor(ParameterExpression originalParam, ParameterExpression replacementParam)
        {
            OriginalParameter = originalParam ?? throw new ArgumentNullException(nameof(originalParam));
            ReplacementParameter = replacementParam ?? throw new ArgumentNullException(nameof(replacementParam));
        }

        protected ParameterExpression OriginalParameter { get; }

        protected ParameterExpression ReplacementParameter { get; }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == OriginalParameter ? ReplacementParameter : base.VisitParameter(node);
        }
    }
}
