using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HEF.Data.Query.Internal
{
    public class EntityQueryProvider : IQueryProvider
    {
        private static readonly MethodInfo _genericCreateQueryMethod
            = typeof(EntityQueryProvider).GetRuntimeMethods()
                .Single(m => (m.Name == "CreateQuery") && m.IsGenericMethod);

        private static readonly MethodInfo _genericExecuteMethod
            = typeof(EntityQueryProvider).GetRuntimeMethods()
                .Single(m => (m.Name == "Execute") && m.IsGenericMethod);

        private readonly IQueryCompiler _queryCompiler;

        public EntityQueryProvider(IQueryCompiler queryCompiler)
        {
            _queryCompiler = queryCompiler ?? throw new ArgumentNullException(nameof(queryCompiler));
        }

        public virtual IQueryable CreateQuery(Expression expression)
            => (IQueryable)_genericCreateQueryMethod
                .MakeGenericMethod(expression.Type.GetSequenceType())
                .Invoke(this, new object[] { expression });

        public virtual IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new EntityQueryable<TElement>(this, expression);

        public virtual object Execute(Expression expression)
            => _genericExecuteMethod.MakeGenericMethod(expression.Type)
                .Invoke(this, new object[] { expression });

        public virtual TResult Execute<TResult>(Expression expression)
            => _queryCompiler.Execute<TResult>(expression);
    }
}
