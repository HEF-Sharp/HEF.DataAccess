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

        #region DaMeng
        public static IServiceProvider DaMengServiceProvider = new ServiceCollection()
            .AddEntityMapperProvider(typeof(DbEntityMapper<>))
            .AddDaMengConnection("server=121.229.172.76;port=35236;user=sysdba;pwd=1hblSQT!@#")
            .AddOracleFormatter()
            .AddSqlBuilder()
            .AddExpressionToOracle()
            .AddEntityQueryable()
            .BuildServiceProvider();

        public static IAsyncQueryProvider DaMengAsyncQueryProvider = DaMengServiceProvider.GetRequiredService<IAsyncQueryProvider>();
        #endregion
    }
}
