using System;
using System.Data;

namespace HEF.Data
{
    public abstract class DbConnectionProvider : IDbConnectionProvider
    {
        public DbConnectionProvider(DbOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(Options.ConnectionString))
                throw new InvalidOperationException("not configured connection string to use.");

            ConnectionString = Options.ConnectionString;
        }

        protected virtual DbOptions Options { get; }

        public string ConnectionString { get; }

        public abstract IDbConnection GetConnection();
    }
}
