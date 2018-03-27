using System;
using System.Data.Common;

namespace HEF.Data.Storage
{
    public abstract class DbConnectionFactory : IDbConnectionFactory
    {
        public DbConnectionFactory(IDbContextOptions contextOptions)
        {
            ContextOptions = contextOptions ?? throw new ArgumentNullException(nameof(contextOptions));
        }

        protected IDbContextOptions ContextOptions { get; }

        public string ConnectionString { get; }

        public abstract DbConnection CreateDbConnection();
    }
}
