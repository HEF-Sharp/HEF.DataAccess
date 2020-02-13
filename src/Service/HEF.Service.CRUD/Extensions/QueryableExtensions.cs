using System;
using System.Linq;

namespace HEF.Service.CRUD
{
    internal static class QueryableExtensions
    {
        internal static IQueryable<TEntity> Action<TEntity>(this IQueryable<TEntity> queryable,
            Action<IQueryable<TEntity>> queryAction)
        {
            if (queryable == null)
                throw new ArgumentNullException(nameof(queryable));

            queryAction?.Invoke(queryable);

            return queryable;
        }
    }
}
