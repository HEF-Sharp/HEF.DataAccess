using HEF.Repository;
using HEF.Repository.Dapper;
using HEF.Sql.Entity;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DapperRepositoryServiceCollectionExtensions
    {
        public static IServiceCollection AddDapperRepository(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddEntityQueryable();

            serviceCollection.TryAddSingleton<IEntitySqlBuilderFactory, EntitySqlBuilderFactory>();
            serviceCollection.TryAddSingleton<IEntityPredicateFactory, EntityPredicateFactory>();

            serviceCollection.TryAddScoped(typeof(IDbRepository<>), typeof(DapperRepository<>));
            serviceCollection.TryAddScoped(typeof(IDbAsyncRepository<>), typeof(DapperRepository<>));
            serviceCollection.TryAddScoped(typeof(IDapperRepository<>), typeof(DapperRepository<>));

            return serviceCollection;
        }
    }
}
