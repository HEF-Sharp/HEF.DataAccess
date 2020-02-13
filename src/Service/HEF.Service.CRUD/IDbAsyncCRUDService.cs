using HEF.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HEF.Service.CRUD
{
    public interface IDbAsyncCRUDService<TEntity> : IDbAsyncService<TEntity>
        where TEntity : class
    {
        #region 查询
        Task<HEFDoResult<TEntity>> GetByKeyAsync(object id);

        Task<HEFDoResult<TEntity>> GetSingleAsync(Action<IQueryable<TEntity>> queryAction);

        Task<HEFDoResult<IList<TEntity>>> GetListAsync(Action<IQueryable<TEntity>> queryAction);
        #endregion

        #region 插入
        Task<HEFDoResult<int>> InsertAsync(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions);
        #endregion

        #region 更新        
        Task<HEFDoResult<int>> UpdateAsync(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions);

        Task<HEFDoResult<int>> UpdateIgnoreAsync(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions);

        Task<HEFDoResult<int>> UpdateByKeyAsync(object id, TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions);

        Task<HEFDoResult<int>> UpdateIgnoreByKeyAsync(object id, TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions);
        #endregion

        #region 删除
        Task<HEFDoResult<int>> DeleteByKeyAsync(object id);

        Task<HEFDoResult<int>> DeleteAsync(TEntity entity);

        Task<HEFDoResult<int>> DeleteByWhereAsync(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions);
        #endregion
    }
}
