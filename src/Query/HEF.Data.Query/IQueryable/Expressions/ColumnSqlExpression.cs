using System;
using System.Linq.Expressions;

namespace HEF.Data.Query
{
    public class ColumnSqlExpression : Expression
    {
        public ColumnSqlExpression(ConstantExpression expression, string alias)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            Alias = alias ?? throw new ArgumentNullException(nameof(alias));
        }

        public ConstantExpression Expression { get; }

        public string Alias { get; }

        public override Type Type => Expression.Type;

        public sealed override ExpressionType NodeType => ExpressionType.Extension;

        public override bool Equals(object obj)
            => obj != null
                && (ReferenceEquals(this, obj)
                    || obj is ColumnSqlExpression columnSqlExpression
                    && Equals(columnSqlExpression));

        private bool Equals(ColumnSqlExpression columnSqlExpression)
            => Expression.Equals(columnSqlExpression.Expression)
                && string.Equals(Alias, columnSqlExpression.Alias);

        public override int GetHashCode() => HashCode.Combine(Expression, Alias);
    }
}
