using HEF.Entity.Mapper;
using HEF.Expressions.Sql;
using HEF.Sql.Formatter;
using HEF.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HEF.Sql.Entity
{
    public class SelectSqlBuilder<TEntity> where TEntity : class
    {
        public SelectSqlBuilder(ISelectSqlBuilder selectSqlBuilder,
            IEntityMapperProvider mapperProvider, IEntitySqlFormatter sqlFormatter,
            IExpressionSqlResolver exprSqlResolver)
        {
            if (mapperProvider == null)
                throw new ArgumentNullException(nameof(mapperProvider));

            Mapper = mapperProvider.GetEntityMapper<TEntity>();

            SqlBuilder = selectSqlBuilder ?? throw new ArgumentNullException(nameof(selectSqlBuilder));
            SqlFormatter = sqlFormatter ?? throw new ArgumentNullException(nameof(sqlFormatter));
            ExprSqlResolver = exprSqlResolver ?? throw new ArgumentNullException(nameof(exprSqlResolver));
        }

        public ISelectSqlBuilder SqlBuilder { get; }

        protected IEntityMapper Mapper { get; }

        protected IEntitySqlFormatter SqlFormatter { get; }

        protected IExpressionSqlResolver ExprSqlResolver { get; }

        public SelectSqlBuilder<TEntity> Column(params Expression<Func<TEntity, object>>[] propertyExpressions)
        {
            if (propertyExpressions.IsEmpty())
                throw new ArgumentNullException(nameof(propertyExpressions));

            var selectProperties = GetSelectProperties(false, propertyExpressions);

            SqlBuilder.Select(string.Join(",", selectProperties.Select(p => SqlFormatter.ColumnName(p, true))));

            return this;
        }

        public SelectSqlBuilder<TEntity> ColumnIgnore(params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            var selectProperties = GetSelectProperties(true, ignorePropertyExpressions);

            SqlBuilder.Select(string.Join(",", selectProperties.Select(p => SqlFormatter.ColumnName(p, true))));

            return this;
        }

        public SelectSqlBuilder<TEntity> Table()
        {
            SqlBuilder.From(SqlFormatter.TableName(Mapper));

            return this;
        }

        public SelectSqlBuilder<TEntity> Where(Expression<Func<TEntity, bool>> predicateExpression)
        {
            var sqlSentence = ExprSqlResolver.Resolve(predicateExpression);

            SqlBuilder.Where(sqlSentence.SqlText);
            if (sqlSentence.Parameters.IsNotEmpty())
            {
                foreach(var sqlParam in sqlSentence.Parameters)
                {
                    SqlBuilder.Parameter(sqlParam.ParameterName, sqlParam.Value);
                }
            }

            return this;
        }

        public SelectSqlBuilder<TEntity> GroupBy(params Expression<Func<TEntity, object>>[] propertyExpressions)
        {
            if (propertyExpressions.IsEmpty())
                throw new ArgumentNullException(nameof(propertyExpressions));

            var groupByProperties = GetSelectProperties(false, propertyExpressions);

            SqlBuilder.GroupBy(string.Join(",", groupByProperties.Select(p => SqlFormatter.ColumnName(p))));

            return this;
        }

        public SelectSqlBuilder<TEntity> OrderBy(params Expression<Func<TEntity, object>>[] propertyExpressions)
        {
            if (propertyExpressions.IsEmpty())
                throw new ArgumentNullException(nameof(propertyExpressions));

            var orderByProperties = GetSelectProperties(false, propertyExpressions);

            SqlBuilder.OrderBy(string.Join(",", orderByProperties.Select(p => SqlFormatter.ColumnName(p))));

            return this;
        }

        public SelectSqlBuilder<TEntity> OrderBy(Expression<Func<TEntity, object>> propertyExpression, bool ascending)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));

            var orderByProperty = GetSelectProperties(false, propertyExpression).Single();

            SqlBuilder.OrderBy($"{SqlFormatter.ColumnName(orderByProperty)} {(ascending ? "asc" : "desc")}");

            return this;
        }

        public SelectSqlBuilder<TEntity> ThenBy(Expression<Func<TEntity, object>> propertyExpression, bool ascending)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));

            var orderByProperty = GetSelectProperties(false, propertyExpression).Single();

            SqlBuilder.OrderBy($", {SqlFormatter.ColumnName(orderByProperty)} {(ascending ? "asc" : "desc")}");

            return this;
        }

        public SqlSentence Build() => SqlBuilder.Build();

        /// <summary>
        /// 获取Select属性
        /// </summary>
        /// <param name="isExclude">是否排除</param>
        /// <param name="propertyExpressions"></param>
        /// <returns></returns>
        private IEnumerable<IPropertyMap> GetSelectProperties(bool isExclude,
            params Expression<Func<TEntity, object>>[] propertyExpressions)
        {
            Func<IPropertyMap, bool> selectPredicate = p => !p.Ignored && !p.IsReadOnly;  //排除只读忽略的属性

            return Mapper.GetProperties(selectPredicate, isExclude, propertyExpressions);
        }
    }
}
