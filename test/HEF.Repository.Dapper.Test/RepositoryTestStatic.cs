using DataAccess.TestCommon;
using HEF.Data;
using HEF.Data.MySql;
using HEF.Sql;
using HEF.Sql.Entity;
using HEF.Data.Query.Test;

namespace HEF.Repository.Dapper.Test
{
    public static class RepositoryTestStatic
    {
        public static IEntitySqlBuilderFactory EntitySqlBuilderFactory = new EntitySqlBuilderFactory(QueryTestStatic.SqlBuilderFactory,
            TestStatic.MapperProvider, TestStatic.MySqlFormatter, TestStatic.ExprMySqlResolver);

        public static IEntityPredicateFactory EntityPredicateFactory = new EntityPredicateFactory(TestStatic.MapperProvider);

        public static IDapperRepository<TEntity> GetDapperRepository<TEntity>()
            where TEntity : class
        {
            return new DapperRepository<TEntity>(QueryTestStatic.ConnectionContext,
                EntitySqlBuilderFactory, EntityPredicateFactory, QueryTestStatic.AsyncQueryProvider, TestStatic.MapperProvider);
        }
    }
}
