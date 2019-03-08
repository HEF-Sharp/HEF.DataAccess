using System.Linq.Expressions;

namespace HEF.Data.Query
{
    public interface ICompiledQueryCacheKeyGenerator
    {
        object GenerateCacheKey(Expression query, bool async);
    }
}
