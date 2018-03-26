using HEF.Data.Modules;
using System;
using System.Data;
using System.Linq;

namespace HEF.Data
{
    public class DbContext : IDbContext
    {
        private readonly DbContextOptions _options;
        private IDbConnection _connection;

        public DbContext(DbContextOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        #region Connection
        public IDbConnection Connection => GetDbConnection();

        private IDbConnection GetDbConnection()
        {
            _connection = _connection ?? CreateDbConnection();

            return _connection;
        }

        private IDbConnection CreateDbConnection()
        {
            var connectionFactoryModule = _options.Modules.SingleOrDefault(m => m.ServiceType == typeof(IDbConnectionFactory));
            if (connectionFactoryModule == null)
                throw new ArgumentException("not found any connection factory module");

            return ((IDbConnectionFactory)connectionFactoryModule.Instance).CreateDbConnection();
        }
        #endregion;

        public void Dispose()
        {
            _connection?.Close();
        }
    }
}
