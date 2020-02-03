using HEF.Entity;
using HEF.Entity.Mapper;
using HEF.Expressions.Sql;
using HEF.Sql;
using HEF.Sql.Formatter;
using HEF.Util;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace HEF.Data.Query
{
    public class DbEntityQueryExecutor : IDbEntityQueryExecutor
    {
        public DbEntityQueryExecutor(IQueryableExpressionVisitorFactory expressionVisitorFactory,
            IDbCommandBuilder dbCommandBuilder,
            ISelectSqlBuilderFactory selectSqlBuilderFactory,
            IEntityMapperProvider mapperProvider,
            IEntitySqlFormatter sqlFormatter,
            IExpressionSqlResolver exprSqlResolver)
        {
            if (expressionVisitorFactory == null)
                throw new ArgumentNullException(nameof(expressionVisitorFactory));

            ExpressionVisitor = expressionVisitorFactory.Create();

            CommandBuilder = dbCommandBuilder ?? throw new ArgumentNullException(nameof(dbCommandBuilder));

            SelectSqlBuilderFactory = selectSqlBuilderFactory ?? throw new ArgumentNullException(nameof(selectSqlBuilderFactory));
            MapperProvider = mapperProvider ?? throw new ArgumentNullException(nameof(mapperProvider));
            SqlFormatter = sqlFormatter ?? throw new ArgumentNullException(nameof(sqlFormatter));
            ExprSqlResolver = exprSqlResolver ?? throw new ArgumentNullException(nameof(exprSqlResolver));
        }

        #region Injected Properties
        protected QueryableExpressionVisitor ExpressionVisitor { get; }

        protected IDbCommandBuilder CommandBuilder { get; }

        protected ISelectSqlBuilderFactory SelectSqlBuilderFactory { get; }

        protected IEntityMapperProvider MapperProvider { get; }

        protected IEntitySqlFormatter SqlFormatter { get; }

        protected IExpressionSqlResolver ExprSqlResolver { get; }
        #endregion

        public TResult Execute<TResult>(Expression query)
        {
            var selectExpr = GetQuerySelectExpression(query);
            var mapper = MapperProvider.GetEntityMapper(selectExpr.EntityType);
            var selectProperties = GetSelectProperties(mapper, selectExpr).ToArray();

            var selectSqlBuilder = ConvertToSelectSqlBuilder(mapper, selectExpr);
            var sqlSentence = selectSqlBuilder.Build();

            var elementFactoryExpr = BuildQueryElementFactory(selectExpr.EntityType, selectProperties);

            var queryingEnumerableExpr = Expression.New(
                typeof(DbQueryingEnumerable<>).MakeGenericType(selectExpr.EntityType).GetConstructors()[0],
                Expression.Constant(CommandBuilder),
                Expression.Constant(sqlSentence),
                Expression.Constant(selectProperties, typeof(IReadOnlyList<IPropertyMap>)),
                Expression.Constant(elementFactoryExpr.Compile()));

            var queryExecutorExpr = Expression.Lambda<Func<TResult>>(queryingEnumerableExpr);

            return queryExecutorExpr.Compile().Invoke();
        }

        public TResult ExecuteAsync<TResult>(Expression query, CancellationToken cancellationToken)
        {
            var selectExpr = GetQuerySelectExpression(query);
            var mapper = MapperProvider.GetEntityMapper(selectExpr.EntityType);
            var selectProperties = GetSelectProperties(mapper, selectExpr).ToArray();

            var selectSqlBuilder = ConvertToSelectSqlBuilder(mapper, selectExpr);
            var sqlSentence = selectSqlBuilder.Build();

            var elementFactoryExpr = BuildQueryElementFactory(selectExpr.EntityType, selectProperties);

            var queryingEnumerableExpr = Expression.New(
                typeof(DbQueryingEnumerable<>).MakeGenericType(selectExpr.EntityType).GetConstructors()[0],
                Expression.Constant(CommandBuilder),
                Expression.Constant(sqlSentence),
                Expression.Constant(selectProperties, typeof(IReadOnlyList<IPropertyMap>)),
                Expression.Constant(elementFactoryExpr.Compile()));

            var queryExecutorExpr = Expression.Lambda<Func<TResult>>(queryingEnumerableExpr);

            return queryExecutorExpr.Compile().Invoke();
        }

        #region Helper Functions
        protected virtual SelectExpression GetQuerySelectExpression(Expression query)
        {
            var queryExpr = ExpressionVisitor.Visit(query);

            if (queryExpr is EntityQueryExpression entityQueryExpr)
            {
                return entityQueryExpr.QueryExpression;
            }

            return null;
        }

        protected virtual IEnumerable<IPropertyMap> GetSelectProperties(IEntityMapper mapper, SelectExpression selectExpression)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            //Todo: future get select column from SelectExpression
            return mapper.Properties.Where(SelectPropertyPredicate);
        }

        #region Convert SelectSqlBuilder
        protected virtual ISelectSqlBuilder ConvertToSelectSqlBuilder(IEntityMapper mapper, SelectExpression selectExpression)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (selectExpression == null)
                throw new ArgumentNullException(nameof(selectExpression));

            var sqlBuilder = SelectSqlBuilderFactory.Create();
            
            var selectProperties = GetSelectProperties(mapper, selectExpression);
            var whereSentence = ExprSqlResolver.Resolve(selectExpression.Predicate);
            var orderByStr = string.Join(",", selectExpression.Orderings.Select(ordering => FormatOrderByColumnStr(mapper, ordering)));

            sqlBuilder.Select(string.Join(",", selectProperties.Select(p => SqlFormatter.ColumnName(p, true))))
                .From(SqlFormatter.TableName(mapper))
                .Where(whereSentence.SqlText)
                .OrderBy(orderByStr)
                .Limit(selectExpression.Limit.Value.ParseInt())
                .Offset(selectExpression.Offset.Value.ParseInt());

            if (whereSentence.Parameters.IsNotEmpty())
            {
                foreach(var whereParam in whereSentence.Parameters)
                {
                    sqlBuilder.Parameter(whereParam.ParameterName, whereParam.Value);
                }
            }

            return sqlBuilder;
        }

        protected virtual bool SelectPropertyPredicate(IPropertyMap propertyMap)
            => !propertyMap.Ignored;  //排除忽略的属性

        protected virtual string FormatOrderByColumnStr(IEntityMapper mapper, OrderingExpression orderingExpression)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (orderingExpression == null)
                throw new ArgumentNullException(nameof(orderingExpression));

            var orderByProperty = GetSelectPropertiesByExpression(mapper, orderingExpression.Expression).SingleOrDefault();
            if (orderByProperty == null)
                throw new ArgumentException($"not found orderby property", nameof(orderingExpression));

            return $"{SqlFormatter.ColumnName(orderByProperty)} {(orderingExpression.IsAscending ? "asc" : "desc")}";
        }

        protected virtual IEnumerable<IPropertyMap> GetSelectPropertiesByExpression(IEntityMapper mapper,
            params LambdaExpression[] propertyExpressions)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (propertyExpressions.IsEmpty())
                throw new ArgumentNullException(nameof(propertyExpressions));

            var propertyNames = propertyExpressions.Select(m => m.ParsePropertyName()) ?? Array.Empty<string>();

            return mapper.Properties.Where(SelectPropertyPredicate).Where(p => propertyNames.Contains(p.Name));
        }
        #endregion

        protected virtual LambdaExpression BuildQueryElementFactory(Type elementType,
            params IPropertyMap[] selectProperties)
        {
            var dataReaderParameter = Expression.Parameter(typeof(DbDataReader), "dataReader");
            var propertyIndexMapParameter = Expression.Parameter(typeof(IDictionary<string, int>), "propertyIndexMap");
            
            var elementInstance = Expression.New(elementType);

            var delegateType = Expression.GetFuncType(typeof(DbDataReader),
                typeof(IDictionary<string, int>), elementType);
            return Expression.Lambda(delegateType, elementInstance, dataReaderParameter, propertyIndexMapParameter);
        }
        #endregion
    }
}
