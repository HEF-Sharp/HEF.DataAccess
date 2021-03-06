﻿using HEF.Entity;
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
using System.Reflection;
using System.Threading;

namespace HEF.Data.Query
{
    public class DbEntityQueryExecutor : IDbEntityQueryExecutor
    {
        public DbEntityQueryExecutor(IQueryableExpressionVisitorFactory expressionVisitorFactory,
            IDbCommandBuilderFactory dbCommandBuilderFactory,
            ISqlBuilderFactory sqlBuilderFactory,
            IEntityMapperProvider mapperProvider,
            IEntitySqlFormatter sqlFormatter,
            IExpressionSqlResolver exprSqlResolver,
            IConcurrencyDetector concurrencyDetector)
        {
            ExpressionVisitorFactory = expressionVisitorFactory ?? throw new ArgumentNullException(nameof(expressionVisitorFactory));
            CommandBuilderFactory = dbCommandBuilderFactory ?? throw new ArgumentNullException(nameof(dbCommandBuilderFactory));
            SqlBuilderFactory = sqlBuilderFactory ?? throw new ArgumentNullException(nameof(sqlBuilderFactory));

            MapperProvider = mapperProvider ?? throw new ArgumentNullException(nameof(mapperProvider));
            SqlFormatter = sqlFormatter ?? throw new ArgumentNullException(nameof(sqlFormatter));
            ExprSqlResolver = exprSqlResolver ?? throw new ArgumentNullException(nameof(exprSqlResolver));

            ConcurrencyDetector = concurrencyDetector ?? throw new ArgumentNullException(nameof(concurrencyDetector));
        }

        #region Injected Properties
        protected IQueryableExpressionVisitorFactory ExpressionVisitorFactory { get; }

        protected IDbCommandBuilderFactory CommandBuilderFactory { get; }

        protected ISqlBuilderFactory SqlBuilderFactory { get; }

        protected IEntityMapperProvider MapperProvider { get; }

        protected IEntitySqlFormatter SqlFormatter { get; }

        protected IExpressionSqlResolver ExprSqlResolver { get; }

        protected IConcurrencyDetector ConcurrencyDetector { get; }
        #endregion

        public TResult Execute<TResult>(Expression query)
        {
            var entityQueryExpr = GetEntityQueryExpression(query);
            var selectExpr = entityQueryExpr.QueryExpression;
            var mapper = MapperProvider.GetEntityMapper(selectExpr.EntityType);
            var selectProperties = GetSelectProperties(mapper, selectExpr).ToArray();

            var selectSqlBuilder = ConvertToSelectSqlBuilder(mapper, selectExpr);
            var sqlSentence = selectSqlBuilder.Build();

            var elementFactory = BuildQueryReturnElementFactory(
                entityQueryExpr.ReturnType, selectProperties);

            var queryExecutor = BuildQueryResultFactory<TResult>(entityQueryExpr, sqlSentence,
                elementFactory, selectProperties);

            return queryExecutor.Invoke();
        }

        public TResult ExecuteAsync<TResult>(Expression query, CancellationToken cancellationToken)
        {
            var entityQueryExpr = GetEntityQueryExpression(query);
            var selectExpr = entityQueryExpr.QueryExpression;
            var mapper = MapperProvider.GetEntityMapper(selectExpr.EntityType);
            var selectProperties = GetSelectProperties(mapper, selectExpr).ToArray();

            var selectSqlBuilder = ConvertToSelectSqlBuilder(mapper, selectExpr);
            var sqlSentence = selectSqlBuilder.Build();

            var elementFactory = BuildQueryReturnElementFactory(
                entityQueryExpr.ReturnType, selectProperties);

            var queryExecutor = BuildQueryResultAsyncFactory<TResult>(entityQueryExpr, sqlSentence,
                elementFactory, cancellationToken, selectProperties);

            return queryExecutor.Invoke();
        }

        #region Helper Functions
        protected virtual EntityQueryExpression GetEntityQueryExpression(Expression query)
        {
            var expressionVisitor = ExpressionVisitorFactory.Create();
            var queryExpr = expressionVisitor.Visit(query);

            if (queryExpr is EntityQueryExpression entityQueryExpr)
            {
                return entityQueryExpr;
            }

            throw new ArgumentException(nameof(query), "resolve EntityQueryExpression failed");
        }

        protected virtual IEnumerable<IPropertyMap> GetSelectProperties(IEntityMapper mapper, SelectExpression selectExpression)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (selectExpression == null)
                throw new ArgumentNullException(nameof(selectExpression));

            //Todo: future get select column from SelectExpression
            if (selectExpression.ColumnSql == null)
                return mapper.Properties.Where(SelectPropertyPredicate);

            return Array.Empty<IPropertyMap>();
        }
        #endregion

        #region Convert SelectSqlBuilder
        protected virtual ISelectSqlBuilder ConvertToSelectSqlBuilder(IEntityMapper mapper, SelectExpression selectExpression)
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (selectExpression == null)
                throw new ArgumentNullException(nameof(selectExpression));

            //设置 删除标识 where条件
            CheckAddDeleteFlagPredicate(mapper, selectExpression);

            var sqlBuilder = SqlBuilderFactory.Select();

            var selectProperties = GetSelectProperties(mapper, selectExpression);
            if (selectProperties.IsNotEmpty())
            {
                var orderByStr = string.Join(",", selectExpression.Orderings.Select(ordering
                    => FormatOrderByColumnStr(selectProperties, ordering)));
                sqlBuilder.Select(string.Join(",", selectProperties.Select(p => SqlFormatter.ColumnName(p, true))))
                    .OrderBy(orderByStr);
            }
            else
            {
                var selectSqlStr = GetColumnSqlString(selectExpression);
                sqlBuilder.Select(selectSqlStr);
            }

            sqlBuilder.From(SqlFormatter.TableName(mapper));

            if (selectExpression.Predicate != null)
            {
                var whereSentence = ExprSqlResolver.Resolve(selectExpression.Predicate);

                sqlBuilder.Where(whereSentence.SqlText);

                if (whereSentence.Parameters.IsNotEmpty())
                {
                    foreach (var whereParam in whereSentence.Parameters)
                    {
                        sqlBuilder.Parameter(whereParam.ParameterName, whereParam.Value);
                    }
                }
            }

            if (selectExpression.Limit != null)
                sqlBuilder.Limit(selectExpression.Limit.Value.ParseInt());

            if (selectExpression.Offset != null)
                sqlBuilder.Offset(selectExpression.Offset.Value.ParseInt());

            return sqlBuilder;
        }

        protected static void CheckAddDeleteFlagPredicate(IEntityMapper mapper, SelectExpression selectExpression)
        {
            var deleteFlagProperty = mapper.Properties.Where(p => p.IsDeleteFlag).SingleOrDefault();
            if (deleteFlagProperty != null)
            {
                var deleteFlagPredicate = BuildDeleteFlagPredicate(mapper.EntityType, deleteFlagProperty);

                selectExpression.ApplyPredicate(deleteFlagPredicate);
            }
        }

        protected static LambdaExpression BuildDeleteFlagPredicate(Type entityType, IPropertyMap deleteFlagProperty)
        {
            var entityParameterExpr = Expression.Parameter(entityType, "entity");

            //code: entity.IsDel != 'Y'
            var deleteFlagPropertyExpr = Expression.Property(entityParameterExpr, deleteFlagProperty.PropertyInfo);

            var deleteFlagPropertyType = deleteFlagProperty.PropertyInfo.PropertyType;
            Expression deleteFlagTrueValueExpr = Expression.Constant(deleteFlagProperty.DeleteFlagTrueValue);
            if (deleteFlagProperty.DeleteFlagTrueValue.GetType() != deleteFlagPropertyType)
                deleteFlagTrueValueExpr = Expression.Convert(deleteFlagTrueValueExpr, deleteFlagPropertyType);

            var deleteFlagNotEqualExpr = Expression.NotEqual(deleteFlagPropertyExpr, deleteFlagTrueValueExpr);

            return Expression.Lambda(Expression.GetFuncType(entityType, typeof(bool)),
                deleteFlagNotEqualExpr, entityParameterExpr);
        }

        protected static bool SelectPropertyPredicate(IPropertyMap propertyMap)
            => !propertyMap.Ignored && !propertyMap.IsReadOnly;  //排除只读忽略的属性

        protected virtual string FormatOrderByColumnStr(IEnumerable<IPropertyMap> sourceProperties, OrderingExpression orderingExpression)
        {
            if (sourceProperties.IsEmpty())
                throw new ArgumentNullException(nameof(sourceProperties));

            if (orderingExpression == null)
                throw new ArgumentNullException(nameof(orderingExpression));

            var orderByPropertyName = orderingExpression.Expression.ParsePropertyName() ?? string.Empty;
            var orderByProperty = sourceProperties.SingleOrDefault(p => string.Compare(p.Name, orderByPropertyName) == 0);
            if (orderByProperty == null)
                throw new ArgumentException($"not found orderby property", nameof(orderingExpression));

            return $"{SqlFormatter.ColumnName(orderByProperty)} {(orderingExpression.IsAscending ? "asc" : "desc")}";
        }

        protected virtual string GetColumnSqlString(SelectExpression selectExpression)
        {
            if (selectExpression == null)
                throw new ArgumentNullException(nameof(selectExpression));

            if (selectExpression.ColumnSql == null)
                throw new InvalidOperationException("not set column sql string");

            var columnSqlStr = selectExpression.ColumnSql.Expression.Value.ParseString();

            if (selectExpression.ColumnSql.Alias.IsNullOrWhiteSpace())
                return columnSqlStr;

            return SqlFormatter.Alias(columnSqlStr, selectExpression.ColumnSql.Alias);
        }
        #endregion

        #region QueryReturnElementFactory
        private static readonly MethodInfo _dictGetValueMethod = typeof(IDictionary<string, int>).GetRuntimeMethod(
            nameof(IDictionary<string, int>.TryGetValue), new[] { typeof(string), typeof(int).MakeByRefType() });

        protected static Delegate BuildQueryReturnElementFactory(Type returnType,
            params IPropertyMap[] selectProperties)
        {
            if (selectProperties.IsEmpty())
                return ExpressionDelegateCache.GetExpressionDelegate($"{returnType}_dataReader_factory",
                    key => BuildQueryReturnTypeFactory(returnType));

            return ExpressionDelegateCache.GetExpressionDelegate(
                $"{returnType}_{string.Join(",", selectProperties.Select(m => m.Name))}_dataReader_factory",
                key => BuildQueryReturnEntityFactory(returnType, selectProperties));
        }

        protected static LambdaExpression BuildQueryReturnTypeFactory(Type returnType)
        {
            var dataReaderParameter = Expression.Parameter(typeof(DbDataReader), "dataReader");
            var propertyIndexMapParameter = Expression.Parameter(typeof(IDictionary<string, int>), "propertyIndexMap");

            // dataReader.IsDBNull(0) ? default(ReturnType) : dataReader.GetValue(0)
            var returnValueExpr = CreateGetValueExpression(dataReaderParameter, returnType,
                Expression.Constant(0, typeof(int)));

            var delegateType = Expression.GetFuncType(typeof(DbDataReader), typeof(IDictionary<string, int>), returnType);
            return Expression.Lambda(delegateType, returnValueExpr, dataReaderParameter, propertyIndexMapParameter);
        }

        protected static LambdaExpression BuildQueryReturnEntityFactory(Type entityType,
            params IPropertyMap[] selectProperties)
        {
            var dataReaderParameter = Expression.Parameter(typeof(DbDataReader), "dataReader");
            var propertyIndexMapParameter = Expression.Parameter(typeof(IDictionary<string, int>), "propertyIndexMap");

            // collect the body
            var bodyExprs = new List<Expression>();

            // var entity = new EntityType();
            var entityVariableExpr = Expression.Variable(entityType, "entity");
            var newEntityExpr = Expression.New(entityType);
            var assignEntityVariableExpr = Expression.Assign(entityVariableExpr, newEntityExpr);
            bodyExprs.Add(assignEntityVariableExpr);

            // int propertyIndex = 0;
            var propertyIndexVariableExpr = Expression.Variable(typeof(int), "propertyIndex");
            var assignPropertyIndexExpr = Expression.Assign(propertyIndexVariableExpr, Expression.Constant(0, typeof(int)));
            bodyExprs.Add(assignPropertyIndexExpr);

            if (selectProperties.IsNotEmpty())
            {
                foreach (var selectProperty in selectProperties)
                {
                    // propertyIndexMap.TryGetValue(propertyName, out propertyIndex);
                    var propertyNameExpr = Expression.Constant(selectProperty.Name);
                    var getPropertyIndexExpr = Expression.Call(propertyIndexMapParameter, _dictGetValueMethod,
                        propertyNameExpr, propertyIndexVariableExpr);
                    bodyExprs.Add(getPropertyIndexExpr);

                    // entity.Property = dataReader.IsDBNull(propertyIndex) ? default(PropertyType) : dataReader.GetValue(propertyIndex);
                    var propertyValueExpr = CreateGetPropertyValueExpression(dataReaderParameter,
                        selectProperty, propertyIndexVariableExpr);
                    var propertyExpr = Expression.Property(entityVariableExpr, selectProperty.PropertyInfo);
                    var assignPropertyExpr = Expression.Assign(propertyExpr, propertyValueExpr);
                    bodyExprs.Add(assignPropertyExpr);
                }
            }

            // code: return (EntityType)entity;
            var castResultExpr = Expression.Convert(entityVariableExpr, entityType);
            bodyExprs.Add(castResultExpr);

            // code: { ... }
            var factoryBodyExpr = Expression.Block(
                entityType, /* return type */
                new[] { entityVariableExpr, propertyIndexVariableExpr } /* local variables */,
                bodyExprs /* body expressions */);

            var delegateType = Expression.GetFuncType(typeof(DbDataReader), typeof(IDictionary<string, int>), entityType);
            return Expression.Lambda(delegateType, factoryBodyExpr, dataReaderParameter, propertyIndexMapParameter);
        }

        protected static Expression CreateGetPropertyValueExpression(ParameterExpression dataReaderParameter,
            IPropertyMap selectProperty, Expression propertyIndexExpr)
        {
            // dataReader.IsDBNull(propertyIndex) ? default(PropertyType) : dataReader.GetValue(propertyIndex);
            var propertyType = selectProperty.PropertyInfo.PropertyType;

            return CreateGetValueExpression(dataReaderParameter, propertyType, propertyIndexExpr);
        }

        protected static Expression CreateGetValueExpression(ParameterExpression dataReaderParameter,
            Type valueType, Expression indexExpr)
        {
            // dataReader.IsDBNull(index) ? default(valueType) : dataReader.GetValue(index);
            var getValueMethod = DataReaderMethods.GetDataReaderGetValueMethod(valueType);
            Expression valueExpr = Expression.Call(dataReaderParameter, getValueMethod, indexExpr);

            if (valueType.IsNullableType())
            {
                valueExpr = Expression.Condition(
                    Expression.Call(dataReaderParameter, DataReaderMethods.IsDbNullMethod, indexExpr),
                    Expression.Default(valueType),
                    Expression.Convert(valueExpr, valueType));
            }

            return valueExpr;
        }
        #endregion

        #region QueryResultFactory
        private static readonly MethodInfo _asyncEnumerableSingleAsyncMethod
            = typeof(AsyncEnumerableExtensions).GetTypeInfo()
                .GetDeclaredMethods(nameof(AsyncEnumerableExtensions.SingleAsync))
                .Single(mi => mi.GetParameters().Length == 2);

        private static readonly MethodInfo _asyncEnumerableSingleOrDefaultAsyncMethod
            = typeof(AsyncEnumerableExtensions).GetTypeInfo()
                .GetDeclaredMethods(nameof(AsyncEnumerableExtensions.SingleOrDefaultAsync))
                .Single(mi => mi.GetParameters().Length == 2);

        protected virtual Func<TResult> BuildQueryResultFactory<TResult>(
            EntityQueryExpression entityQueryExpr, SqlSentence sqlSentence,
            Delegate elementFactory,
            params IPropertyMap[] selectProperties)
        {
            var queryEnumerableExpr = BuildQueryEnumerableExpression(entityQueryExpr, sqlSentence,
                elementFactory, selectProperties);

            var queryResultExpr = queryEnumerableExpr;
            if (entityQueryExpr.ReturnSingle)
            {
                var singleMethod = entityQueryExpr.ReturnDefault
                    ? EnumerableMethods.SingleOrDefaultWithoutPredicate
                    : EnumerableMethods.SingleWithoutPredicate;

                queryResultExpr = Expression.Call(
                    singleMethod.MakeGenericMethod(entityQueryExpr.ReturnType),
                    queryEnumerableExpr);
            }

            return Expression.Lambda<Func<TResult>>(queryResultExpr).Compile();
        }

        protected virtual Func<TResult> BuildQueryResultAsyncFactory<TResult>(
            EntityQueryExpression entityQueryExpr, SqlSentence sqlSentence,
            Delegate elementFactory,
            CancellationToken cancellationToken,
            params IPropertyMap[] selectProperties)
        {
            var queryEnumerableExpr = BuildQueryEnumerableExpression(entityQueryExpr, sqlSentence,
                elementFactory, selectProperties);

            var queryResultExpr = queryEnumerableExpr;
            if (entityQueryExpr.ReturnSingle)
            {
                var singleAsyncMethod = entityQueryExpr.ReturnDefault
                    ? _asyncEnumerableSingleOrDefaultAsyncMethod
                    : _asyncEnumerableSingleAsyncMethod;

                queryResultExpr = Expression.Call(
                    singleAsyncMethod.MakeGenericMethod(entityQueryExpr.ReturnType),
                    queryEnumerableExpr,
                    Expression.Constant(cancellationToken));
            }

            return Expression.Lambda<Func<TResult>>(queryResultExpr).Compile();
        }

        protected virtual Expression BuildQueryEnumerableExpression(
            EntityQueryExpression entityQueryExpr, SqlSentence sqlSentence,
            Delegate elementFactory,
            params IPropertyMap[] selectProperties)
        {
            var queryingEnumerableExpr = Expression.New(
                typeof(DbQueryingEnumerable<>).MakeGenericType(entityQueryExpr.ReturnType).GetConstructors()[0],
                Expression.Constant(CommandBuilderFactory.Create()),
                Expression.Constant(sqlSentence),
                Expression.Constant(selectProperties, typeof(IReadOnlyList<IPropertyMap>)),
                Expression.Constant(elementFactory),
                Expression.Constant(ConcurrencyDetector));

            return queryingEnumerableExpr;
        }
        #endregion
    }
}
