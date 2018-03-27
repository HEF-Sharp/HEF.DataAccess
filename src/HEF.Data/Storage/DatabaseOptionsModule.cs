using System;
using Microsoft.Extensions.DependencyInjection;

namespace HEF.Data.Storage
{
    public abstract class DatabaseOptionsModule : IDbContextOptionsModule
    {
        private string _connectionString;

        protected DatabaseOptionsModule()
        { }

        protected DatabaseOptionsModule(DatabaseOptionsModule copyFrom)
        {
            if (copyFrom == null)
                throw new ArgumentNullException(nameof(copyFrom));

            _connectionString = copyFrom._connectionString;
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

        public abstract bool ApplyServices(IServiceCollection services);
    }
}
