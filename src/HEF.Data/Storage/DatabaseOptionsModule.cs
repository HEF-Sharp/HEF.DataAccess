using System;
using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;

namespace HEF.Data.Storage
{
    public abstract class DatabaseOptionsModule : IDbContextOptionsModule
    {
        private string _connectionString;
        private DbConnection _connection;
        private int? _commandTimeout;

        protected DatabaseOptionsModule()
        { }

        protected DatabaseOptionsModule(DatabaseOptionsModule copyFrom)
        {
            if (copyFrom == null)
                throw new ArgumentNullException(nameof(copyFrom));

            _connectionString = copyFrom._connectionString;
            _connection = copyFrom._connection;
            _commandTimeout = copyFrom._commandTimeout;
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

        public virtual int? CommandTimeout => _commandTimeout;

        public virtual DatabaseOptionsModule WithCommandTimeout(int? commandTimeout)
        {
            if (commandTimeout.HasValue && commandTimeout <= 0)
            {
                throw new InvalidOperationException("The specified CommandTimeout value is not valid. It must be a positive number.");
            }

            var clone = Clone();

            clone._commandTimeout = commandTimeout;

            return clone;
        }

        public abstract bool ApplyServices(IServiceCollection services);
    }
}
