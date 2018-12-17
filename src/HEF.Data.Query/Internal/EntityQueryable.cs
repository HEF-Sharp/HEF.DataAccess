using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HEF.Data.Query.Internal
{
    public class EntityQueryable<TResult> : IOrderedQueryable<TResult>
    {
        private readonly IQueryProvider _queryProvider;

        public EntityQueryable(IQueryProvider queryProvider)
        {
            _queryProvider = queryProvider ?? throw new ArgumentNullException(nameof(queryProvider));
            Expression = Expression.Constant(this);
        }

        public EntityQueryable(IQueryProvider queryProvider, Expression expression)
        {
            _queryProvider = queryProvider ?? throw new ArgumentNullException(nameof(queryProvider));
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public virtual Type ElementType => typeof(TResult);

        public virtual Expression Expression { get; }

        public virtual IQueryProvider Provider => _queryProvider;

        public virtual IEnumerator<TResult> GetEnumerator()
            => _queryProvider.Execute<IEnumerable<TResult>>(Expression).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _queryProvider.Execute<IEnumerable>(Expression).GetEnumerator();
    }
}
