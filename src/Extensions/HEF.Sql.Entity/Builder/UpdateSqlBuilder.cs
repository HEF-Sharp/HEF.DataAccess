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
    public class UpdateSqlBuilder<TEntity> : ISqlBuilder
        where TEntity : class
    {
        public UpdateSqlBuilder(IUpdateSqlBuilder updateSqlBuilder,
            IEntityMapperProvider mapperProvider, IEntitySqlFormatter sqlFormatter,
            IExpressionSqlResolver exprSqlResolver)
        {
            if (mapperProvider == null)
                throw new ArgumentNullException(nameof(mapperProvider));           

            Mapper = mapperProvider.GetEntityMapper<TEntity>();

            SqlBuilder = updateSqlBuilder ?? throw new ArgumentNullException(nameof(updateSqlBuilder));
            SqlFormatter = sqlFormatter ?? throw new ArgumentNullException(nameof(sqlFormatter));
            ExprSqlResolver = exprSqlResolver ?? throw new ArgumentNullException(nameof(exprSqlResolver));
        }

        public IUpdateSqlBuilder SqlBuilder { get; }

        protected IEntityMapper Mapper { get; }

        protected IEntitySqlFormatter SqlFormatter { get; }

        protected IExpressionSqlResolver ExprSqlResolver { get; }

        protected Expression<Func<TEntity, bool>> PredicateExpr { get; private set; }

        public UpdateSqlBuilder<TEntity> Table()
        {
            SqlBuilder.Table(SqlFormatter.TableName(Mapper));

            return this;
        }

        public UpdateSqlBuilder<TEntity> Column(TEntity entity, params Expression<Func<TEntity, object>>[] propertyExpressions)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (propertyExpressions.IsEmpty())
                throw new ArgumentNullException(nameof(propertyExpressions));

            var updateProperties = GetUpdateProperties(false, propertyExpressions).ToList();

            updateProperties.ForEach(property => MappingEntityProperty(entity, property));

            return this;
        }

        public UpdateSqlBuilder<TEntity> ColumnIgnore(TEntity entity, params Expression<Func<TEntity, object>>[] ignorePropertyExpressions)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var updateProperties = GetUpdateProperties(true, ignorePropertyExpressions).ToList();

            updateProperties.ForEach(property => MappingEntityProperty(entity, property));

            return this;
        }

        public UpdateSqlBuilder<TEntity> ColumnDeleteFlag()
        {
            var deleteFlagProperty = Mapper.GetDeleteFlagProperty();
            if (deleteFlagProperty == null)
                throw new InvalidOperationException("not found deleteFlag propety in entity");

            SqlBuilder.Column(SqlFormatter.ColumnName(deleteFlagProperty),
                SqlFormatter.Parameter(deleteFlagProperty.Name), deleteFlagProperty.DeleteFlagTrueValue);

            return this;
        }

        public UpdateSqlBuilder<TEntity> Where(Expression<Func<TEntity, bool>> predicateExpression)
        {
            if (predicateExpression == null)
                throw new ArgumentNullException(nameof(predicateExpression));

            if (predicateExpression.Body is ConstantExpression predicateConstant
                && (bool)predicateConstant.Value)
            {
                return this;
            }

            PredicateExpr = PredicateExpr.CombinePredicate(predicateExpression);

            return this;
        }

        public SqlSentence Build() => ResolveWhereSqlAndParameters().SqlBuilder.Build();
        
        /// <summary>
        /// 获取Update属性
        /// </summary>
        /// <param name="isExclude">是否排除</param>
        /// <param name="propertyExpressions"></param>
        /// <returns></returns>
        private IEnumerable<IPropertyMap> GetUpdateProperties(bool isExclude,
            params Expression<Func<TEntity, object>>[] propertyExpressions)
        {
            Func<IPropertyMap, bool> updatePredicate =
                p => !p.Ignored && !p.IsReadOnly && p.KeyType == KeyType.NotAKey;  //排除主键和只读忽略的属性

            return Mapper.GetProperties(updatePredicate, isExclude, propertyExpressions);
        }

        private void MappingEntityProperty(TEntity entity, IPropertyMap propertyMap)
        {
            var propertyValue = propertyMap.PropertyInfo.GetValue(entity);

            SqlBuilder.Column(SqlFormatter.ColumnName(propertyMap), SqlFormatter.Parameter(propertyMap.Name), propertyValue);
        }

        private UpdateSqlBuilder<TEntity> ResolveWhereSqlAndParameters()
        {
            if (PredicateExpr == null)
                return this;

            var sqlSentence = ExprSqlResolver.Resolve(PredicateExpr);

            SqlBuilder.Where(sqlSentence.SqlText);
            if (sqlSentence.Parameters.IsNotEmpty())
            {
                foreach (var sqlParam in sqlSentence.Parameters)
                {
                    SqlBuilder.Parameter(sqlParam.ParameterName, sqlParam.Value);
                }
            }

            return this;
        }
    }
}
