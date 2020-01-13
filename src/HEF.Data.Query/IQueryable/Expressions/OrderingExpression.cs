using System;
using System.Linq.Expressions;

namespace HEF.Data.Query
{
    public class OrderingExpression : Expression
    {
        public OrderingExpression(Expression expression, bool ascending)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            IsAscending = ascending;
        }

        public Expression Expression { get; }

        public bool IsAscending { get; }

        public override Type Type => Expression.Type;

        public sealed override ExpressionType NodeType => ExpressionType.Extension;        

        public override bool Equals(object obj)
            => obj != null
                && (ReferenceEquals(this, obj)
                    || obj is OrderingExpression orderingExpression
                    && Equals(orderingExpression));

        private bool Equals(OrderingExpression orderingExpression)
            => Expression.Equals(orderingExpression.Expression)
                && IsAscending == orderingExpression.IsAscending;

        public override int GetHashCode() => HashCode.Combine(Expression, IsAscending);
    }
}
