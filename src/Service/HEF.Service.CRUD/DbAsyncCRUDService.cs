using HEF.Core;
using HEF.Repository;
using HEF.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HEF.Service.CRUD
{
    public class DbAsyncCRUDService<TEntity> : DbAsyncService<TEntity>, IDbAsyncCRUDService<TEntity>
        where TEntity : class
    {
        public DbAsyncCRUDService(IDbAsyncRepository<TEntity> repository)
            : base(repository)
        { }

        #region 查询
        public virtual async Task<HEFDoResult<TEntity>> GetByKeyAsync(object id)
        {
            var entity = await Repository.GetByKeyAsync(id);

            return entity == null ? HEFDoResultHelper.DoNotFound<TEntity>($"not find the record of pkid: {id}")
                : HEFDoResultHelper.DoSuccess(entity);
        }

        public virtual async Task<HEFDoResult<TEntity>> GetSingleAsync(Action<IQueryable<TEntity>> queryAction)
        {
            var queryable = await Repository.QueryAsync();
            var result = queryable.Action(queryAction).ToList().Single();

            return result == null ? HEFDoResultHelper.DoNotFound<TEntity>("not found any record")
               : HEFDoResultHelper.DoSuccess(result);
        }

        public virtual async Task<HEFDoResult<IList<TEntity>>> GetListAsync(Action<IQueryable<TEntity>> queryAction)
        {
            var queryable = await Repository.QueryAsync();
            var results = queryable.Action(queryAction).ToList();

            return results.IsEmpty() ? HEFDoResultHelper.DoNotFound<IList<TEntity>>("not found any record")
               : HEFDoResultHelper.DoSuccess<IList<TEntity>>(results);
        }
        #endregion

        #region 插入
        public virtual async Task<HEFDoResult<int>> InsertAsync(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            var validResult = entity.Validate<TEntity, int>(ignorePropertyExpressions);
            if (validResult.IsValidFail)
                return validResult;  //验证失败

            var result = await Repository.InsertAsync(entity, ignorePropertyExpressions);

            return result > 0 ? HEFDoResultHelper.DoFail<int>("insert fail") : HEFDoResultHelper.DoSuccess(result);
        }
        #endregion

        #region 更新
        public virtual async Task<HEFDoResult<int>> UpdateAsync(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
        {
            var validResult = entity.ValidateInclude<TEntity, int>(includePropertyExpressions);
            if (validResult.IsValidFail)
                return validResult;  //验证失败

            var count = await Repository.UpdateAsync(entity, includePropertyExpressions);

            return count > 0 ? HEFDoResultHelper.DoSuccess(count) : HEFDoResultHelper.DoFail<int>("update fail");
        }

        public virtual async Task<HEFDoResult<int>> UpdateIgnoreAsync(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            var validResult = entity.Validate<TEntity, int>(ignorePropertyExpressions);
            if (validResult.IsValidFail)
                return validResult;  //验证失败

            var count = await Repository.UpdateIgnoreAsync(entity, ignorePropertyExpressions);

            return count > 0 ? HEFDoResultHelper.DoSuccess(count) : HEFDoResultHelper.DoFail<int>("update fail");
        }

        public virtual async Task<HEFDoResult<int>> UpdateByKeyAsync(object id, TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
        {
            var validResult = entity.ValidateInclude<TEntity, int>(includePropertyExpressions);
            if (validResult.IsValidFail)
                return validResult;  //验证失败

            var count = await Repository.UpdateByKeyAsync(id, entity, includePropertyExpressions);

            return count > 0 ? HEFDoResultHelper.DoSuccess(count) : HEFDoResultHelper.DoFail<int>("update fail");
        }

        public virtual async Task<HEFDoResult<int>> UpdateIgnoreByKeyAsync(object id, TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            var validResult = entity.Validate<TEntity, int>(ignorePropertyExpressions);
            if (validResult.IsValidFail)
                return validResult;  //验证失败

            var count = await Repository.UpdateIgnoreByKeyAsync(id, entity, ignorePropertyExpressions);

            return count > 0 ? HEFDoResultHelper.DoSuccess(count) : HEFDoResultHelper.DoFail<int>("update fail");
        }
        #endregion

        #region 删除
        public virtual async Task<HEFDoResult<int>> DeleteByKeyAsync(object id)
        {
            var count = await Repository.DeleteByKeyAsync(id);

            return count > 0 ? HEFDoResultHelper.DoSuccess(count) : HEFDoResultHelper.DoFail<int>("delete fail");
        }

        public virtual async Task<HEFDoResult<int>> DeleteAsync(TEntity entity)
        {
            var count = await Repository.DeleteAsync(entity);

            return count > 0 ? HEFDoResultHelper.DoSuccess(count) : HEFDoResultHelper.DoFail<int>("delete fail");
        }

        public virtual async Task<HEFDoResult<int>> DeleteByWhereAsync(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
        {
            var count = await Repository.DeleteByWhereAsync(entity, includePropertyExpressions);

            return count > 0 ? HEFDoResultHelper.DoSuccess(count) : HEFDoResultHelper.DoFail<int>("delete fail");
        }
        #endregion
    }
}
