using System;
using System.Collections.Generic;
using System.Linq;

namespace HEF.Data.Query
{
    public static class QueryableExtensions
    {
        public static IAsyncEnumerable<TSource> AsAsyncEnumerable<TSource>(
            this IQueryable<TSource> queryable)
        {
            if (queryable == null)
                throw new ArgumentNullException(nameof(queryable));

            if (queryable is IAsyncEnumerable<TSource> asyncEnumerable)
            {
                return asyncEnumerable;
            }

            throw new InvalidOperationException("The source IQueryable doesn't implement IAsyncEnumerable");
        }
    }
}
