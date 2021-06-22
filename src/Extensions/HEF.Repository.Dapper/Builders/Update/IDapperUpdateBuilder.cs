using System;
using System.Linq.Expressions;

namespace HEF.Repository.Dapper
{
    public interface IDapperUpdateBuilder<TEntity> : IDapperExecute
        where TEntity : class
    {
        IDapperUpdateBuilder<TEntity> Key(object id);

        IDapperUpdateBuilder<TEntity> Key(TEntity entity);

        IDapperUpdateBuilder<TEntity> Columns(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions);

        IDapperUpdateBuilder<TEntity> ColumnsIgnore(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions);

        IDapperUpdateBuilder<TEntity> Wheres(TEntity entity, params Expression<Func<TEntity, object>>[] wherePropertyExpressions);
    }
}
