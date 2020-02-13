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
            var selectExpr = GetQuerySelectExpression(query);
            var mapper = MapperProvider.GetEntityMapper(selectExpr.EntityType);
            var selectProperties = GetSelectProperties(mapper, selectExpr).ToArray();

            var selectSqlBuilder = ConvertToSelectSqlBuilder(mapper, selectExpr);
            var sqlSentence = selectSqlBuilder.Build();

            var elementFactoryExpr = BuildQueryElementFactory(selectExpr.EntityType, selectProperties);

            var queryingEnumerableExpr = Expression.New(
                typeof(DbQueryingEnumerable<>).MakeGenericType(selectExpr.EntityType).GetConstructors()[0],
                Expression.Constant(CommandBuilderFactory.Create()),
                Expression.Constant(sqlSentence),
                Expression.Constant(selectProperties, typeof(IReadOnlyList<IPropertyMap>)),
                Expression.Constant(elementFactoryExpr.Compile()),
                Expression.Constant(ConcurrencyDetector));

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
                Expression.Constant(CommandBuilderFactory.Create()),
                Expression.Constant(sqlSentence),
                Expression.Constant(selectProperties, typeof(IReadOnlyList<IPropertyMap>)),
                Expression.Constant(elementFactoryExpr.Compile()),
                Expression.Constant(ConcurrencyDetector));

            var queryExecutorExpr = Expression.Lambda<Func<TResult>>(queryingEnumerableExpr);

            return queryExecutorExpr.Compile().Invoke();
        }

        #region Helper Functions
        protected virtual SelectExpression GetQuerySelectExpression(Expression query)
        {
            var expressionVisitor = ExpressionVisitorFactory.Create();
            var queryExpr = expressionVisitor.Visit(query);

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
            var whereSentence = ExprSqlResolver.Resolve(selectExpression.Predicate);
            var orderByStr = string.Join(",", selectExpression.Orderings.Select(ordering
                => FormatOrderByColumnStr(selectProperties, ordering)));

            sqlBuilder.Select(string.Join(",", selectProperties.Select(p => SqlFormatter.ColumnName(p, true))))
                .From(SqlFormatter.TableName(mapper))
                .Where(whereSentence.SqlText)
                .OrderBy(orderByStr);

            if (selectExpression.Limit != null)
                sqlBuilder.Limit(selectExpression.Limit.Value.ParseInt());

            if (selectExpression.Offset != null)
                sqlBuilder.Offset(selectExpression.Offset.Value.ParseInt());

            if (whereSentence.Parameters.IsNotEmpty())
            {
                foreach (var whereParam in whereSentence.Parameters)
                {
                    sqlBuilder.Parameter(whereParam.ParameterName, whereParam.Value);
                }
            }

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
        #endregion

        #region QueryElementFactory
        private static readonly MethodInfo _dictGetValueMethod = typeof(IDictionary<string, int>).GetRuntimeMethod(
            nameof(IDictionary<string, int>.TryGetValue), new[] { typeof(string), typeof(int).MakeByRefType() });

        protected static LambdaExpression BuildQueryElementFactory(Type elementType,
            params IPropertyMap[] selectProperties)
        {
            var dataReaderParameter = Expression.Parameter(typeof(DbDataReader), "dataReader");
            var propertyIndexMapParameter = Expression.Parameter(typeof(IDictionary<string, int>), "propertyIndexMap");

            // collect the body
            var bodyExprs = new List<Expression>();

            // var element = new ElementType();
            var elementVariableExpr = Expression.Variable(elementType, "element");
            var newElementExpr = Expression.New(elementType);
            var assignElementVariableExpr = Expression.Assign(elementVariableExpr, newElementExpr);
            bodyExprs.Add(assignElementVariableExpr);

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

                    // element.Property = dataReader.IsDBNull(propertyIndex) ? default(PropertyType) : dataReader.GetValue(propertyIndex);
                    var propertyValueExpr = CreateGetPropertyValueExpression(dataReaderParameter,
                        selectProperty, propertyIndexVariableExpr);
                    var propertyExpr = Expression.Property(elementVariableExpr, selectProperty.PropertyInfo);
                    var assignPropertyExpr = Expression.Assign(propertyExpr, propertyValueExpr);
                    bodyExprs.Add(assignPropertyExpr);
                }
            }

            // code: return (ElementType)element;
            var castResultExpr = Expression.Convert(elementVariableExpr, elementType);
            bodyExprs.Add(castResultExpr);

            // code: { ... }
            var factoryBodyExpr = Expression.Block(
                elementType, /* return type */
                new[] { elementVariableExpr, propertyIndexVariableExpr } /* local variables */,
                bodyExprs /* body expressions */);

            var delegateType = Expression.GetFuncType(typeof(DbDataReader), typeof(IDictionary<string, int>), elementType);
            return Expression.Lambda(delegateType, factoryBodyExpr, dataReaderParameter, propertyIndexMapParameter);
        }

        protected static Expression CreateGetPropertyValueExpression(ParameterExpression dataReaderParameter,
            IPropertyMap selectProperty, Expression propertyIndexExpr)
        {
            // dataReader.IsDBNull(propertyIndex) ? default(PropertyType) : dataReader.GetValue(propertyIndex);
            var propertyType = selectProperty.PropertyInfo.PropertyType;
            var getPropertyValueMethod = DataReaderMethods.GetDataReaderGetValueMethod(propertyType);
            Expression propertyValueExpr = Expression.Call(dataReaderParameter, getPropertyValueMethod, propertyIndexExpr);

            if (propertyType.IsNullableType())
            {
                propertyValueExpr = Expression.Condition(
                    Expression.Call(dataReaderParameter, DataReaderMethods.IsDbNullMethod, propertyIndexExpr),
                    Expression.Default(propertyType),
                    propertyValueExpr);
            }

            return propertyValueExpr;
        }
        #endregion
    }
}
