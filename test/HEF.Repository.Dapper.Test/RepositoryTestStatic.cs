using DataAccess.TestCommon;
using HEF.Data;
using HEF.Data.MySql;
using HEF.Sql;
using HEF.Sql.Entity;

namespace HEF.Repository.Dapper.Test
{
    public static class RepositoryTestStatic
    {
        public static IDbConnectionProvider ConnectionProvider = new MySqlConnectionProvider(
            new DbOptions().WithConnectionString("server=localhost;port=3306;database=develop_test;uid=root;pwd=aaacdb0074"));

        public static IDbConnectionContext ConnectionContext = new DbConnectionContext(ConnectionProvider);

        public static ISqlBuilderFactory SqlBuilderFactory = new SqlBuilderFactory();

        public static IEntitySqlBuilderFactory EntitySqlBuilderFactory = new EntitySqlBuilderFactory(SqlBuilderFactory,
            TestStatic.MapperProvider, TestStatic.MySqlFormatter, TestStatic.ExprMySqlResolver);

        public static IEntityPredicateFactory EntityPredicateFactory = new EntityPredicateFactory(TestStatic.MapperProvider);

        public static IDapperRepository<TEntity> GetDapperRepository<TEntity>()
            where TEntity : class
        {
            return new DapperRepository<TEntity>(ConnectionContext,
                EntitySqlBuilderFactory, EntityPredicateFactory, TestStatic.MapperProvider);
        }
    }
}
