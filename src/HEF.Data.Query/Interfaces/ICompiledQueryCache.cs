using System;

namespace HEF.Data.Query
{
    public interface ICompiledQueryCache
    {
        Func<QueryContext, TResult> GetOrAddQuery<TResult>(
            object cacheKey,
            Func<Func<QueryContext, TResult>> compiler);
       
        Func<QueryContext, TResult> GetOrAddAsyncQuery<TResult>(
            object cacheKey,
            Func<Func<QueryContext, TResult>> compiler);
    }
}
