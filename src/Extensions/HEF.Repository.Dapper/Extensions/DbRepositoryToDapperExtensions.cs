using System;

namespace HEF.Repository.Dapper
{
    public static class DbRepositoryToDapperExtensions
    {
        public static IDapperRepository<TEntity> AsDapper<TEntity>(this IDbRepository<TEntity> dbRepository)
            where TEntity : class
        {
            if (dbRepository == null)
                throw new ArgumentNullException(nameof(dbRepository));

            if (dbRepository is IDapperRepository<TEntity> dapperRepository)
                return dapperRepository;

            throw new InvalidCastException("convert to dapper repository failed");
        }

        public static IDapperRepository<TEntity> AsDapper<TEntity>(this IDbAsyncRepository<TEntity> dbAsyncRepository)
            where TEntity : class
        {
            if (dbAsyncRepository == null)
                throw new ArgumentNullException(nameof(dbAsyncRepository));

            if (dbAsyncRepository is IDapperRepository<TEntity> dapperRepository)
                return dapperRepository;

            throw new InvalidCastException("convert to dapper repository failed");
        }
    }
}
