using HEF.Entity.Mapper;
using HEF.Expressions.Sql;
using HEF.Sql.Formatter;
using HEF.Util;
using System;
using System.Linq.Expressions;

namespace HEF.Sql.Entity
{
    public class DeleteSqlBuilder<TEntity> : ISqlBuilder
        where TEntity : class
    {
        public DeleteSqlBuilder(IDeleteSqlBuilder deleteSqlBuilder,
            IEntityMapperProvider mapperProvider, IEntitySqlFormatter sqlFormatter,
            IExpressionSqlResolver exprSqlResolver)
        {
            if (mapperProvider == null)
                throw new ArgumentNullException(nameof(mapperProvider));            

            Mapper = mapperProvider.GetEntityMapper<TEntity>();

            SqlBuilder = deleteSqlBuilder ?? throw new ArgumentNullException(nameof(deleteSqlBuilder));
            SqlFormatter = sqlFormatter ?? throw new ArgumentNullException(nameof(sqlFormatter));
            ExprSqlResolver = exprSqlResolver ?? throw new ArgumentNullException(nameof(exprSqlResolver));
        }

        public IDeleteSqlBuilder SqlBuilder { get; }

        protected IEntityMapper Mapper { get; }

        protected IEntitySqlFormatter SqlFormatter { get; }

        protected IExpressionSqlResolver ExprSqlResolver { get; }

        protected Expression<Func<TEntity, bool>> PredicateExpr { get; private set; }

        public DeleteSqlBuilder<TEntity> Table()
        {
            SqlBuilder.Table(SqlFormatter.TableName(Mapper));

            return this;
        }

        public DeleteSqlBuilder<TEntity> Where(Expression<Func<TEntity, bool>> predicateExpression)
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

        private DeleteSqlBuilder<TEntity> ResolveWhereSqlAndParameters()
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
