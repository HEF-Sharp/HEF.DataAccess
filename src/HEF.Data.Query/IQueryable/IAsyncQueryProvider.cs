using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace HEF.Data.Query
{
    public interface IAsyncQueryProvider : IQueryProvider
    {
        TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default);
    }
}
