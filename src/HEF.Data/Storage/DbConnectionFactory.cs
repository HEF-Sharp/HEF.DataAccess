using System;
using System.Data.Common;
using HEF.Data.Extensions;

namespace HEF.Data.Storage
{
    public abstract class DbConnectionFactory : IDbConnectionFactory
    {
        public DbConnectionFactory(IDbContextOptions contextOptions,
            INamedConnectionStringResolver connectionStringResolver)
        {
            ContextOptions = contextOptions ?? throw new ArgumentNullException(nameof(contextOptions));

            if (connectionStringResolver == null)
                throw new ArgumentNullException(nameof(connectionStringResolver));

            var databaseOptions = contextOptions.FindDatabaseOptions();

            if (string.IsNullOrWhiteSpace(databaseOptions.ConnectionString))
                throw new InvalidOperationException("database has been configured without specifying connection string to use.");

            ConnectionString = connectionStringResolver.ResolveConnectionString(databaseOptions.ConnectionString);
        }

        protected IDbContextOptions ContextOptions { get; }

        public string ConnectionString { get; }

        public abstract DbConnection CreateDbConnection();
    }
}
