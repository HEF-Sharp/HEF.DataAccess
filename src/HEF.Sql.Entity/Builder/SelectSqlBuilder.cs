using HEF.Entity;
using HEF.Entity.Mapper;
using HEF.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HEF.Sql
{
    public class SelectSqlBuilder<TEntity> : SelectSqlBuilder where TEntity : class
    {
        public SelectSqlBuilder(IEntityMapperProvider mapperProvider, IEntitySqlFormatter sqlFormatter)
        {
            if (mapperProvider == null)
                throw new ArgumentNullException(nameof(mapperProvider));

            if (sqlFormatter == null)
                throw new ArgumentNullException(nameof(sqlFormatter));

            Mapper = mapperProvider.GetEntityMapper<TEntity>();

            SqlFormatter = sqlFormatter;
        }

        protected IEntityMapper Mapper { get; }

        protected IEntitySqlFormatter SqlFormatter { get; }

        public SelectSqlBuilder<TEntity> Column(params Expression<Func<TEntity, object>>[] propertyExpressions)
        {
            if (propertyExpressions.IsEmpty())
                throw new ArgumentNullException(nameof(propertyExpressions));

            var selectProperties = GetSelectProperties(false, propertyExpressions);

            Select(string.Join(",", selectProperties.Select(p => SqlFormatter.ColumnName(p))));

            return this;
        }

        public SelectSqlBuilder<TEntity> ColumnIgnore(params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            var selectProperties = GetSelectProperties(true, ignorePropertyExpressions);

            Select(string.Join(",", selectProperties.Select(p => SqlFormatter.ColumnName(p))));

            return this;
        }

        public SelectSqlBuilder<TEntity> Table()
        {
            From(SqlFormatter.TableName(Mapper));

            return this;
        }

        public SelectSqlBuilder<TEntity> Where(Expression<Func<TEntity, bool>> predicateExpression)
        {
            throw new NotImplementedException();
        }

        public SelectSqlBuilder<TEntity> GroupBy(params Expression<Func<TEntity, object>>[] propertyExpressions)
        {
            if (propertyExpressions.IsEmpty())
                throw new ArgumentNullException(nameof(propertyExpressions));

            var groupByProperties = GetSelectProperties(false, propertyExpressions);

            GroupBy(string.Join(",", groupByProperties.Select(p => SqlFormatter.ColumnName(p))));

            return this;
        }

        public SelectSqlBuilder<TEntity> OrderBy(params Expression<Func<TEntity, object>>[] propertyExpressions)
        {
            if (propertyExpressions.IsEmpty())
                throw new ArgumentNullException(nameof(propertyExpressions));

            var orderByProperties = GetSelectProperties(false, propertyExpressions);

            OrderBy(string.Join(",", orderByProperties.Select(p => SqlFormatter.ColumnName(p))));

            return this;
        }

        /// <summary>
        /// 获取Select属性
        /// </summary>
        /// <param name="isExclude">是否排除</param>
        /// <param name="propertyExpressions"></param>
        /// <returns></returns>
        private IEnumerable<IPropertyMap> GetSelectProperties(bool isExclude,
            params Expression<Func<TEntity, object>>[] propertyExpressions)
        {            
            var selectProperties = Mapper.Properties.Where(p => !p.Ignored);  //排除忽略的属性

            var propertyNames = propertyExpressions.Select(m => m.ParsePropertyName()) ?? Array.Empty<string>();

            Func<string, bool> propertyPredicate = (name) => propertyNames.Contains(name);
            if (isExclude)
            {
                propertyPredicate = (name) => !propertyNames.Contains(name);
            }

            selectProperties = selectProperties.Where(p => propertyPredicate(p.Name));

            return selectProperties;
        }
    }
}
