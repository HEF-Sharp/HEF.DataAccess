using System;
using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;

namespace HEF.Data.Storage
{
    public abstract class DatabaseOptionsModule : IDbContextOptionsModule
    {
        private string _connectionString;
        private DbConnection _connection;        

        protected DatabaseOptionsModule()
        { }

        protected DatabaseOptionsModule(DatabaseOptionsModule copyFrom)
        {
            if (copyFrom == null)
                throw new ArgumentNullException(nameof(copyFrom));

            _connectionString = copyFrom._connectionString;
            _connection = copyFrom._connection;
        }

        protected abstract DatabaseOptionsModule Clone();

        public virtual string ConnectionString => _connectionString;        

        public virtual DatabaseOptionsModule WithConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(connectionString);

            var clone = Clone();

            clone._connectionString = connectionString;

            return clone;
        }

        public virtual DbConnection Connection => _connection;

        public virtual DatabaseOptionsModule WithConnection(DbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            var clone = Clone();

            clone._connection = connection;

            return clone;
        }

        public virtual bool ApplyServices(IServiceCollection services)
        {
            services.AddScoped<INamedConnectionStringResolver, NamedConnectionStringResolver>();

            return false;
        }

        public virtual long GetServiceProviderHashCode() => 0;
    }
}
