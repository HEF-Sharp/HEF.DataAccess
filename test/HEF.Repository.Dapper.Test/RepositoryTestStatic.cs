using DataAccess.TestCommon;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HEF.Repository.Dapper.Test
{
    public static class RepositoryTestStatic
    {
        public static IServiceProvider MySqlServiceProvider = new ServiceCollection()
            .AddEntityMapperProvider(typeof(DbEntityMapper<>))
            .AddMySqlConnection("server=localhost;port=3306;database=develop_test;uid=root;pwd=aaacdb0074")
            .AddMySqlFormatter()
            .AddSqlBuilder()
            .AddExpressionToMySql()
            .AddDapperRepository()            
            .BuildServiceProvider();

        public static IDapperRepository<TEntity> GetDapperRepository<TEntity>()
            where TEntity : class
        {
            return MySqlServiceProvider.GetRequiredService<IDapperRepository<TEntity>>();
        }
    }
}
