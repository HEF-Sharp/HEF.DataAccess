using DataAccess.TestCommon;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HEF.Data.Query.Test
{
    public static class QueryTestStatic
    {
        public static IServiceProvider MySqlServiceProvider = new ServiceCollection()
            .AddEntityMapperProvider(typeof(DbEntityMapper<>))
            .AddMySqlConnection("server=localhost;port=3306;database=develop_test;uid=root;pwd=aaacdb0074")
            .AddMySqlFormatter()
            .AddSqlBuilder()
            .AddExpressionToMySql()
            .AddEntityQueryable()
            .BuildServiceProvider();

        public static IAsyncQueryProvider AsyncQueryProvider = MySqlServiceProvider.GetRequiredService<IAsyncQueryProvider>();
    }
}
