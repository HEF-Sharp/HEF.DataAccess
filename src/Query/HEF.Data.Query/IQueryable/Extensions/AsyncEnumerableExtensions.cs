using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HEF.Data.Query
{
    internal static class AsyncEnumerableExtensions
    {
        internal static async Task<TSource> SingleAsync<TSource>(
            this IAsyncEnumerable<TSource> asyncEnumerable,
            CancellationToken cancellationToken = default)
        {
            if (asyncEnumerable == null)
                throw new ArgumentNullException(nameof(asyncEnumerable));

            await using var enumerator = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
            if (!await enumerator.MoveNextAsync())
                throw new InvalidOperationException("Sequence contains no elements");

            var result = enumerator.Current;

            if (await enumerator.MoveNextAsync())
                throw new InvalidOperationException("Sequence contains more than one element");

            return result;
        }

        internal static async Task<TSource> SingleOrDefaultAsync<TSource>(
            this IAsyncEnumerable<TSource> asyncEnumerable,
            CancellationToken cancellationToken = default)
        {
            if (asyncEnumerable == null)
                throw new ArgumentNullException(nameof(asyncEnumerable));

            await using var enumerator = asyncEnumerable.GetAsyncEnumerator(cancellationToken);
            if (!await enumerator.MoveNextAsync())
                return default;

            var result = enumerator.Current;

            if (await enumerator.MoveNextAsync())
                throw new InvalidOperationException("Sequence contains more than one element");

            return result;
        }
    }
}
