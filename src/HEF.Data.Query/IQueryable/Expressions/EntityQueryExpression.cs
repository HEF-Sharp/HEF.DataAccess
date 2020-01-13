using System;
using System.Linq;
using System.Linq.Expressions;

namespace HEF.Data.Query
{
    public class EntityQueryExpression : Expression
    {
        internal EntityQueryExpression(Type entityType, bool querySingle = false)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            Type = GetExpressionType(entityType, querySingle);
            QueryExpression = new SelectExpression(entityType);
        }

        public Expression QueryExpression { get; }

        public override Type Type { get; }

        public sealed override ExpressionType NodeType => ExpressionType.Extension;

        protected virtual Type GetExpressionType(Type entityType, bool querySingle)
            => querySingle ? entityType : typeof(IQueryable<>).MakeGenericType(entityType);
    }
}
