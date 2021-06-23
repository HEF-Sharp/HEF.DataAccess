using System;
using System.Linq.Expressions;

namespace HEF.Repository.Dapper
{
    public interface IDapperInsertBuilder<TEntity> : IDapperExecute
        where TEntity : class
    {
        IDapperInsertBuilder<TEntity> Columns(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions);

        IDapperInsertBuilder<TEntity> ColumnsIgnore(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions);
    }
}
