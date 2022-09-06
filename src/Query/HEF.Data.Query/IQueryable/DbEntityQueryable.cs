using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace HEF.Data.Query
{
    public class DbEntityQueryable<TElement> : IOrderedQueryable<TElement>, IAsyncEnumerable<TElement>
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

        public virtual Type ElementType => typeof(TElement);

        public virtual Expression Expression { get; }

        public virtual IQueryProvider Provider => _queryProvider;

        public virtual IEnumerator<TElement> GetEnumerator()
            => _queryProvider.Execute<IEnumerable<TElement>>(Expression).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _queryProvider.Execute<IEnumerable>(Expression).GetEnumerator();

        public virtual IAsyncEnumerator<TElement> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => _queryProvider.ExecuteAsync<IAsyncEnumerable<TElement>>(Expression).GetAsyncEnumerator(cancellationToken);
    }
}
