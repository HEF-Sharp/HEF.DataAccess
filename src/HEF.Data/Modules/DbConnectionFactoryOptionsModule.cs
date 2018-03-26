using System;

namespace HEF.Data.Modules
{
    public abstract class DbConnectionFactoryOptionsModule : IDbContextOptionsModule
    {
        private string _connectionString;

        protected DbConnectionFactoryOptionsModule()
        { }

        protected DbConnectionFactoryOptionsModule(DbConnectionFactoryOptionsModule copyFrom)
        {
            if (copyFrom == null)
                throw new ArgumentNullException(nameof(copyFrom));

            _connectionString = copyFrom._connectionString;
        }

        protected abstract DbConnectionFactoryOptionsModule Clone();

        public virtual string ConnectionString => _connectionString;

        public Type ServiceType => typeof(IDbConnectionFactory);

        public abstract object Instance { get; }

        public virtual DbConnectionFactoryOptionsModule WithConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(connectionString);

            var clone = Clone();

            clone._connectionString = connectionString;

            return clone;
        }
    }
}
