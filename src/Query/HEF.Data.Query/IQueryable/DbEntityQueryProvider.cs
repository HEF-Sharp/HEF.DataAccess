using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace HEF.Data.Query
{
    public class DbEntityQueryProvider : IAsyncQueryProvider
    {
        private static readonly MethodInfo _genericCreateQueryMethod
            = typeof(DbEntityQueryProvider).GetRuntimeMethods()
                .Single(m => (m.Name == "CreateQuery") && m.IsGenericMethod);

        private static MethodInfo _genericExecuteMethod
            = typeof(DbEntityQueryProvider).GetRuntimeMethods()
                .Single(m => (m.Name == "Execute") && m.IsGenericMethod);        

        public DbEntityQueryProvider(IDbEntityQueryExecutor queryExecutor)
        {
            QueryExecutor = queryExecutor ?? throw new ArgumentNullException(nameof(queryExecutor));
        }

        protected IDbEntityQueryExecutor QueryExecutor { get; }

        public virtual IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new DbEntityQueryable<TElement>(this, expression);

        public virtual IQueryable CreateQuery(Expression expression)
            => (IQueryable)_genericCreateQueryMethod
                .MakeGenericMethod(expression.Type.GetSequenceType())
                .Invoke(this, new object[] { expression });

        public virtual TResult Execute<TResult>(Expression expression)
            => QueryExecutor.Execute<TResult>(expression);

        public virtual object Execute(Expression expression)
            => _genericExecuteMethod.MakeGenericMethod(expression.Type)
                .Invoke(this, new object[] { expression });

        public virtual TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
            => QueryExecutor.ExecuteAsync<TResult>(expression, cancellationToken);
    }
}
