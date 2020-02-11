using HEF.Core;
using HEF.Repository;
using System;
using System.Linq.Expressions;

namespace HEF.Service.CRUD
{
    public class DbCRUDService<TEntity> : DbService<TEntity>, IDbCRUDService<TEntity>
        where TEntity : class
    {
        public DbCRUDService(IDbRepository<TEntity> repository)
            : base(repository)
        { }

        #region 查询
        public virtual HEFDoResult<TEntity> GetByKey(object id)
        {
            var entity = Repository.GetByKey(id);

            return entity == null ? HEFDoResultHelper.DoNotFound<TEntity>($"not find the record of pkid: {id}")
                : HEFDoResultHelper.DoSuccess(entity);
        }
        #endregion

        #region 插入
        public virtual HEFDoResult<int> Insert(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            var validResult = entity.Validate<TEntity, int>(ignorePropertyExpressions);
            if (validResult.IsValidFail)
                return validResult;  //验证失败

            var result = Repository.Insert(entity, ignorePropertyExpressions);

            return result > 0 ? HEFDoResultHelper.DoFail<int>("insert fail") : HEFDoResultHelper.DoSuccess(result);
        }
        #endregion

        #region 更新
        public virtual HEFDoResult<int> Update(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
        {
            var validResult = entity.ValidateInclude<TEntity, int>(includePropertyExpressions);
            if (validResult.IsValidFail)
                return validResult;  //验证失败

            var count = Repository.Update(entity, includePropertyExpressions);

            return count > 0 ? HEFDoResultHelper.DoSuccess(count) : HEFDoResultHelper.DoFail<int>("update fail");
        }

        public virtual HEFDoResult<int> UpdateIgnore(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            var validResult = entity.Validate<TEntity, int>(ignorePropertyExpressions);
            if (validResult.IsValidFail)
                return validResult;  //验证失败

            var count = Repository.UpdateIgnore(entity, ignorePropertyExpressions);

            return count > 0 ? HEFDoResultHelper.DoSuccess(count) : HEFDoResultHelper.DoFail<int>("update fail");
        }

        public virtual HEFDoResult<int> UpdateByKey(object id, TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
        {
            var validResult = entity.ValidateInclude<TEntity, int>(includePropertyExpressions);
            if (validResult.IsValidFail)
                return validResult;  //验证失败

            var count = Repository.UpdateByKey(id, entity, includePropertyExpressions);

            return count > 0 ? HEFDoResultHelper.DoSuccess(count) : HEFDoResultHelper.DoFail<int>("update fail");
        }

        public virtual HEFDoResult<int> UpdateIgnoreByKey(object id, TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            var validResult = entity.Validate<TEntity, int>(ignorePropertyExpressions);
            if (validResult.IsValidFail)
                return validResult;  //验证失败

            var count = Repository.UpdateIgnoreByKey(id, entity, ignorePropertyExpressions);

            return count > 0 ? HEFDoResultHelper.DoSuccess(count) : HEFDoResultHelper.DoFail<int>("update fail");
        }
        #endregion

        #region 删除
        public virtual HEFDoResult<int> DeleteByKey(object id)
        {
            var count = Repository.DeleteByKey(id);

            return count > 0 ? HEFDoResultHelper.DoSuccess(count) : HEFDoResultHelper.DoFail<int>("delete fail");
        }

        public virtual HEFDoResult<int> Delete(TEntity entity)
        {
            var count = Repository.Delete(entity);

            return count > 0 ? HEFDoResultHelper.DoSuccess(count) : HEFDoResultHelper.DoFail<int>("delete fail");
        }

        public virtual HEFDoResult<int> DeleteByWhere(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions)
        {
            var count = Repository.DeleteByWhere(entity, includePropertyExpressions);

            return count > 0 ? HEFDoResultHelper.DoSuccess(count) : HEFDoResultHelper.DoFail<int>("delete fail");
        }
        #endregion
    }
}
