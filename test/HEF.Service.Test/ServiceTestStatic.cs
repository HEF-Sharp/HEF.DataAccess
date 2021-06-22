using DataAccess.TestCommon;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HEF.Service.Test
{
    public static class ServiceTestStatic
    {
        public static IServiceProvider MySqlServiceProvider = new ServiceCollection()
            .AddEntityMapperProvider(typeof(DbEntityMapper<>))
            .AddMySqlConnection("server=localhost;port=3306;database=develop_test;uid=root;pwd=aaacdb0074")
            .AddMySqlFormatter()
            .AddSqlBuilder()
            .AddExpressionToMySql()
            .AddDapperRepository()
            .AddDbService()
            .BuildServiceProvider();

        public static IDbService<TEntity> GetDbService<TEntity>()
            where TEntity : class
        {
            return MySqlServiceProvider.GetRequiredService<IDbService<TEntity>>();
        }

        public static IDbAsyncService<TEntity> GetDbAsyncService<TEntity>()
            where TEntity : class
        {
            return MySqlServiceProvider.GetRequiredService<IDbAsyncService<TEntity>>();
        }
    }
}
