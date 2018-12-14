using System;
using System.Linq.Expressions;

namespace HEF.Data.Query
{
    public interface IQueryCompiler
    {
        TResult Execute<TResult>(Expression query);

        Func<QueryContext, TResult> CreateCompiledQuery<TResult>(Expression query);
    }
}
