using System;
using System.Linq.Expressions;

namespace HEF.Expressions
{
    public abstract class ExpressionResolver : ExpressionVisitor
    {
        public override Expression Visit(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            
            if (IsResolveNodeType(expression.NodeType))
                return base.Visit(expression);

            throw new NotSupportedException($"The LINQ expression node of type {expression.NodeType} is not supported");
        }

        protected abstract bool IsResolveNodeType(ExpressionType nodeType);
    }
}
