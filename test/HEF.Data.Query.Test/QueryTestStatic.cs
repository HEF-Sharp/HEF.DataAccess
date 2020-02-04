using DataAccess.TestCommon;
using HEF.Data.MySql;

namespace HEF.Data.Query.Test
{
    public static class QueryTestStatic
    {
        public static IQueryableExpressionVisitorFactory QueryableExprVistiorFactory = new QueryableExpressionVisitorFactory();

        public static IDbConnectionProvider ConnectionProvider = new MySqlConnectionProvider(
            new DbOptions().WithConnectionString("server=localhost;port=3306;database=develop_test;uid=root;pwd=aaacdb0074"));

        public static IDbConnectionContext ConnectionContext = new DbConnectionContext(ConnectionProvider);

        public static IDbCommandBuilder CommandBuilder = new DbCommandBuilder(ConnectionContext);

        public static ISelectSqlBuilderFactory SelectSqlBuilderFactory = new SelectSqlBuilderFactory();

        public static IConcurrencyDetector ConcurrencyDetector = new ConcurrencyDetector();

        public static IDbEntityQueryExecutor EntityQueryExecutor = new DbEntityQueryExecutor(QueryableExprVistiorFactory,
            CommandBuilder, SelectSqlBuilderFactory,
            TestStatic.MapperProvider, TestStatic.MySqlFormatter, TestStatic.ExprMySqlResolver, ConcurrencyDetector);

        public static IAsyncQueryProvider AsyncQueryProvider = new DbEntityQueryProvider(EntityQueryExecutor);
    }
}
