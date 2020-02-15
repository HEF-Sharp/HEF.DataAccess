using System;
using System.Linq;
using System.Linq.Expressions;

namespace HEF.Data.Query
{
    public class EntityQueryExpression : Expression
    {
        internal EntityQueryExpression(Type entityType)
            : this(entityType, entityType, null, false)
        { }

        private EntityQueryExpression(Type entityType, Type returnType,
            SelectExpression queryExpression,
            bool returnSingle, bool returnDefault = false)
        {
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
            ReturnType = returnType ?? throw new ArgumentNullException(nameof(returnType));
            ReturnSingle = returnSingle;
            ReturnDefault = returnDefault;

            Type = GetQueryExpressionType(ReturnType, ReturnSingle);
            QueryExpression = queryExpression ?? new SelectExpression(EntityType);
        }

        public SelectExpression QueryExpression { get; }

        public override Type Type { get; }

        public sealed override ExpressionType NodeType => ExpressionType.Extension;

        #region Internal Properties
        internal Type EntityType { get; }

        internal Type ReturnType { get; }

        internal bool ReturnSingle { get; }

        internal bool ReturnDefault { get; }
        #endregion

        protected static Type GetQueryExpressionType(Type returnType, bool returnSingle)
            => returnSingle ? returnType : typeof(IQueryable<>).MakeGenericType(returnType);

        internal virtual EntityQueryExpression QuerySingle(bool returnDefault)
        {
            return new EntityQueryExpression(EntityType, ReturnType,
                QueryExpression, true, returnDefault);
        }

        internal virtual EntityQueryExpression QueryReturn(Type returnType)
        {
            if (ReturnType == returnType)
                return this;

            return new EntityQueryExpression(EntityType, returnType,
                QueryExpression, ReturnSingle, ReturnDefault);
        }
    }
}
