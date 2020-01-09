using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace HEF.Data.Query
{
    public class DbEntityQueryable<TResult> : IOrderedQueryable<TResult>, IAsyncEnumerable<TResult>
    {
        private readonly IAsyncQueryProvider _queryProvider;

        public DbEntityQueryable(IAsyncQueryProvider queryProvider)
        {
            _queryProvider = queryProvider ?? throw new ArgumentNullException(nameof(queryProvider));
            Expression = Expression.Constant(this);
        }

        public DbEntityQueryable(IAsyncQueryProvider queryProvider, Expression expression)
        {
            _queryProvider = queryProvider ?? throw new ArgumentNullException(nameof(queryProvider));
            Expression = expression;
        }

        public virtual Type ElementType => typeof(TResult);

        public virtual Expression Expression { get; }

        public virtual IQueryProvider Provider => _queryProvider;

        public virtual IEnumerator<TResult> GetEnumerator()
            => _queryProvider.Execute<IEnumerable<TResult>>(Expression).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _queryProvider.Execute<IEnumerable>(Expression).GetEnumerator();

        public virtual IAsyncEnumerator<TResult> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => _queryProvider.ExecuteAsync<IAsyncEnumerable<TResult>>(Expression).GetAsyncEnumerator(cancellationToken);
    }
}
