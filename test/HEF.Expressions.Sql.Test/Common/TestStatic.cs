using HEF.Entity.Mapper;
using HEF.Sql;
using HEF.Sql.Entity;

namespace HEF.Expressions.Sql.Test
{
    public static class TestStatic
    {
        public static IEntityMapperProvider MapperProvider = new EntityMapperProvider(typeof(DbEntityMapper<>));

        public static IEntitySqlFormatter SqlFormatter = new EntitySqlFormatter(new SqlFormatter());

        public static IEntitySqlFormatter MySqlFormatter = new EntitySqlFormatter(new MySqlFormatter());

        public static IMethodCallSqlResolver MethodSqlResolver = new MethodCallSqlResolver();

        public static IExpressionSqlResolver ExprSqlResolver = new ExpressionSqlResolver(MapperProvider, SqlFormatter, MethodSqlResolver);

        public static IExpressionSqlResolver ExprMySqlResolver = new ExpressionSqlResolver(MapperProvider, MySqlFormatter, MethodSqlResolver);
    }
}
