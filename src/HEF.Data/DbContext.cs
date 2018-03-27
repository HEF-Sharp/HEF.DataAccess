using System;
using System.Data;

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
            throw new NotImplementedException();
        }
        #endregion;

        public void Dispose()
        {
            _connection?.Close();
        }
    }
}
