using System;
using System.Data.Common;

namespace HEF.Data.Storage
{
    public abstract class DbConnectionProvider : IDbConnectionProvider
    {
        private readonly string _connectionString;
        private readonly Lazy<DbConnection> _connection;
        private readonly bool _connectionOwned;

        public DbConnectionProvider(IDbContextOptions contextOptions,
            INamedConnectionStringResolver connectionStringResolver)
        {
            ContextOptions = contextOptions ?? throw new ArgumentNullException(nameof(contextOptions));

            if (connectionStringResolver == null)
                throw new ArgumentNullException(nameof(connectionStringResolver));

            var databaseOptions = contextOptions.FindDatabaseOptions();

            if (databaseOptions.Connection != null)
            {
                if (!string.IsNullOrWhiteSpace(databaseOptions.ConnectionString))                
                    throw new InvalidOperationException("Both an existing DbConnection and a connection string have been configured");                

                _connection = new Lazy<DbConnection>(() => databaseOptions.Connection);
                _connectionOwned = false;
            }
            else if (!string.IsNullOrWhiteSpace(databaseOptions.ConnectionString))
            {
                _connectionString = connectionStringResolver.ResolveConnectionString(databaseOptions.ConnectionString);
                _connection = new Lazy<DbConnection>(CreateDbConnection);
                _connectionOwned = true;
            }
            else
            {
                throw new InvalidOperationException("database has been configured without specifying either the DbConnection or connection string to use.");
            }
        }

        protected virtual IDbContextOptions ContextOptions { get; }

        protected abstract DbConnection CreateDbConnection();        

        public virtual string ConnectionString => _connectionString ?? Connection.ConnectionString;

        public virtual DbConnection Connection => _connection.Value;

        public virtual void Dispose()
        {
            if (_connectionOwned && _connection.IsValueCreated)
            {
                Connection.Dispose();
            }
        }
    }
}
