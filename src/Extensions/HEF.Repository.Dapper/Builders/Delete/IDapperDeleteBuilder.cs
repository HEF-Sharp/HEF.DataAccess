using System;
using System.Linq.Expressions;

namespace HEF.Repository.Dapper
{
    public interface IDapperDeleteBuilder<TEntity> : IDapperExecute
        where TEntity : class
    {
        IDapperDeleteBuilder<TEntity> Key(object id);

        IDapperDeleteBuilder<TEntity> Key(TEntity entity);

        IDapperDeleteBuilder<TEntity> Wheres(TEntity entity, params Expression<Func<TEntity, object>>[] wherePropertyExpressions);
    }
}
