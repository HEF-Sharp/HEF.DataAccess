using HEF.Entity.Mapper;
using System;
using System.Linq.Expressions;

namespace HEF.Sql.Entity
{
    public class DeleteSqlBuilder<TEntity> where TEntity : class
    {
        public DeleteSqlBuilder(IDeleteSqlBuilder deleteSqlBuilder,
            IEntityMapperProvider mapperProvider, IEntitySqlFormatter sqlFormatter)
        {
            if (deleteSqlBuilder == null)
                throw new ArgumentNullException(nameof(deleteSqlBuilder));

            if (mapperProvider == null)
                throw new ArgumentNullException(nameof(mapperProvider));

            if (sqlFormatter == null)
                throw new ArgumentNullException(nameof(sqlFormatter));

            SqlBuilder = deleteSqlBuilder;

            Mapper = mapperProvider.GetEntityMapper<TEntity>();

            SqlFormatter = sqlFormatter;
        }

        public IDeleteSqlBuilder SqlBuilder { get; }

        protected IEntityMapper Mapper { get; }

        protected IEntitySqlFormatter SqlFormatter { get; }

        public DeleteSqlBuilder<TEntity> Table()
        {
            SqlBuilder.Table(SqlFormatter.TableName(Mapper));

            return this;
        }

        public DeleteSqlBuilder<TEntity> Where(Expression<Func<TEntity, bool>> predicateExpression)
        {
            throw new NotImplementedException();
        }
    }
}
