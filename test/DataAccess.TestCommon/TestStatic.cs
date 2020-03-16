using HEF.Expressions.Sql;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DataAccess.TestCommon
{
    public static class TestStatic
    {
        public static IServiceProvider SqlServiceProvider = new ServiceCollection()
            .AddEntityMapperProvider(typeof(DbEntityMapper<>))
            .AddSqlFormatter()
            .AddExpressionToSql()
            .BuildServiceProvider();

        public static IServiceProvider MySqlServiceProvider = new ServiceCollection()
            .AddEntityMapperProvider(typeof(DbEntityMapper<>))
            .AddMySqlFormatter()
            .AddExpressionToMySql()
            .BuildServiceProvider();
        
        public static IExpressionSqlResolver ExprSqlResolver = SqlServiceProvider.GetRequiredService<IExpressionSqlResolver>();

        public static IExpressionSqlResolver ExprMySqlResolver = MySqlServiceProvider.GetRequiredService<IExpressionSqlResolver>();
    }
}
