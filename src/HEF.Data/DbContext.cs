using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;

namespace HEF.Data
{
    public class DbContext : IDbContext
    {
        private readonly DbContextOptions _options;

        private IServiceScope _serviceScope;
        private IDbConnection _connection;

        private bool _disposed;

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

        private IServiceProvider InternalServiceProvider
        {
            get
            {
                CheckDisposed();

                if (_serviceScope != null)
                    return _serviceScope.ServiceProvider;

                throw new NotImplementedException();
            }
        }

        #region Dispose Support
        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name, "Cannot access a disposed context object.");
            }
        }

        public virtual void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                _serviceScope?.Dispose();
                _connection?.Close();
            }
        }
        #endregion
    }
}
