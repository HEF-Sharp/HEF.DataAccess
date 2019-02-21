using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;

namespace HEF.Data.Query.Internal
{
    public class CompiledQueryCache : ICompiledQueryCache
    {
        public const string CompiledQueryParameterPrefix = "__";

        private static readonly ConcurrentDictionary<object, object> _querySyncObjects
            = new ConcurrentDictionary<object, object>();

        private readonly IMemoryCache _memoryCache;

        public CompiledQueryCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public virtual Func<QueryContext, TResult> GetOrAddQuery<TResult>(
            object cacheKey, Func<Func<QueryContext, TResult>> compiler)
            => GetOrAddQueryCore(cacheKey, compiler);

        public virtual Func<QueryContext, TResult> GetOrAddAsyncQuery<TResult>(
            object cacheKey, Func<Func<QueryContext, TResult>> compiler)
            => GetOrAddQueryCore(cacheKey, compiler);

        private Func<QueryContext, TFunc> GetOrAddQueryCore<TFunc>(
            object cacheKey, Func<Func<QueryContext, TFunc>> compiler)
        {
            retry:
            if (!_memoryCache.TryGetValue(cacheKey, out Func<QueryContext, TFunc> compiledQuery))
            {
                if (!_querySyncObjects.TryAdd(cacheKey, value: null))
                {
                    goto retry;
                }

                try
                {
                    compiledQuery = compiler();

                    _memoryCache.Set(cacheKey, compiledQuery);
                }
                finally
                {
                    _querySyncObjects.TryRemove(cacheKey, out _);
                }
            }

            return compiledQuery;
        }
    }
}
