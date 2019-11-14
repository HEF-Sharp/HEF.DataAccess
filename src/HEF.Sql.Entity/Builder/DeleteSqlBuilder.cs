using HEF.Entity.Mapper;
using System;
using System.Linq.Expressions;

namespace HEF.Sql.Entity
{
    public class DeleteSqlBuilder<TEntity> : DeleteSqlBuilder where TEntity : class
    {
        public DeleteSqlBuilder(IEntityMapperProvider mapperProvider, IEntitySqlFormatter sqlFormatter)
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

        public DeleteSqlBuilder<TEntity> Table()
        {
            Table(SqlFormatter.TableName(Mapper));

            return this;
        }

        public DeleteSqlBuilder<TEntity> Where(Expression<Func<TEntity, bool>> predicateExpression)
        {
            throw new NotImplementedException();
        }
    }
}
