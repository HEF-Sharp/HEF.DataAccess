using Remotion.Linq;
using System.Linq.Expressions;

namespace HEF.Data.Query
{
    public interface IQueryModelGenerator
    {
        QueryModel ParseQuery(Expression query);
    }
}
