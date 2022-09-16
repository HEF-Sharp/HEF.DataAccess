using HEF.Core;
using System;
using System.Linq;

namespace HEF.Service.CRUD
{
    public static class QueryableExtensions
    {
        public static IQueryable<TEntity> Action<TEntity>(this IQueryable<TEntity> queryable,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> queryAction)
        {
            if (queryable == null)
                throw new ArgumentNullException(nameof(queryable));

            if (queryAction != null)
                return queryAction.Invoke(queryable);

            return queryable;
        }

        public static HEFPageData<TEntity> GetPageList<TEntity>(this IQueryable<TEntity> queryable,
            int currentPage, int pageSize)
            where TEntity : class
        {
            if (queryable == null)
                throw new ArgumentNullException(nameof(queryable));

            if (currentPage < 1)
                throw new ArgumentOutOfRangeException(nameof(currentPage), "current page must greater than zero");

            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "page size must greater than zero");

            var pageData = new HEFPageData<TEntity>() { PageIndex = currentPage, PageSize = pageSize };

            pageData.Data = queryable.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            pageData.Total = queryable.Count();

            return pageData;
        }
    }
}
