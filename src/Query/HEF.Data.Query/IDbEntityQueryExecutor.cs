using System.Linq.Expressions;
using System.Threading;

namespace HEF.Data.Query
{
    public interface IDbEntityQueryExecutor
    {
        TResult Execute<TResult>(Expression query);

        TResult ExecuteAsync<TResult>(Expression query, CancellationToken cancellationToken);
    }
}
