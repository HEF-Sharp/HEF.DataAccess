﻿using HEF.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HEF.Service.CRUD
{
    public interface IDbCRUDService<TEntity> : IDbService<TEntity>
        where TEntity : class
    {
        #region 查询
        HEFDoResult<TEntity> GetByKey(object id);

        HEFDoResult<TEntity> GetSingleOrDefault(Func<IQueryable<TEntity>, IQueryable<TEntity>> queryAction);

        HEFDoResult<IList<TEntity>> GetList(Func<IQueryable<TEntity>, IQueryable<TEntity>> queryAction);

        HEFDoResult<HEFPageData<TEntity>> GetPageList(int currentPage, int pageSize,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> queryAction);
        #endregion

        #region 插入
        HEFDoResult<int> Insert(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions);
        #endregion

        #region 更新        
        HEFDoResult<int> Update(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions);

        HEFDoResult<int> UpdateIgnore(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions);

        HEFDoResult<int> UpdateByKey(object id, TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions);

        HEFDoResult<int> UpdateIgnoreByKey(object id, TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions);
        #endregion

        #region 删除
        HEFDoResult<int> DeleteByKey(object id);

        HEFDoResult<int> Delete(TEntity entity);

        HEFDoResult<int> DeleteByWhere(TEntity entity, params Expression<Func<TEntity, object>>[] includePropertyExpressions);
        #endregion
    }
}