using System;
using System.Linq.Expressions;

namespace HEF.Data.Query.Internal
{
    public class CompiledQueryCacheKeyGenerator : ICompiledQueryCacheKeyGenerator
    {
        public virtual object GenerateCacheKey(Expression query, bool async)
            => GenerateCacheKeyCore(query, async);

        protected CompiledQueryCacheKey GenerateCacheKeyCore(Expression query, bool async)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            return new CompiledQueryCacheKey(query, async);
        }

        protected readonly struct CompiledQueryCacheKey
        {
            private readonly Expression _query;            
            private readonly bool _async;

            /// <summary>
            ///     Initializes a new instance of the <see cref="CompiledQueryCacheKey" /> class.
            /// </summary>
            /// <param name="query"> The query to generate the key for. </param>            
            /// <param name="async"> A value indicating whether the query will be executed asynchronously. </param>
            public CompiledQueryCacheKey(
                Expression query,
                bool async)
            {
                _query = query;
                _async = async;
            }

            /// <summary>
            ///     Determines if this key is equivalent to a given object (i.e. if they are keys for the same query).
            /// </summary>
            /// <param name="obj">
            ///     The object to compare this key to.
            /// </param>
            /// <returns>
            ///     True if the object is a <see cref="CompiledQueryCacheKey" /> and is for the same query, otherwise false.
            /// </returns>
            public override bool Equals(object obj)
            {
                if (obj is null
                    || !(obj is CompiledQueryCacheKey))
                {
                    return false;
                }

                var other = (CompiledQueryCacheKey)obj;

                return _async == other._async
                       && ExpressionEqualityComparer.Instance.Equals(_query, other._query);
            }

            /// <summary>
            ///     Gets the hash code for the key.
            /// </summary>
            /// <returns>
            ///     The hash code for the key.
            /// </returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = ExpressionEqualityComparer.Instance.GetHashCode(_query);                   
                    hashCode = (hashCode * 397) ^ _async.GetHashCode();
                    return hashCode;
                }
            }
        }
    }
}
