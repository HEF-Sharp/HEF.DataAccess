using HEF.Data.Internal;
using HEF.Data.Storage;
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
            var connectionFactory = InternalServiceProvider.GetRequiredService<IDbConnectionFactory>();

            return connectionFactory.CreateDbConnection();
        }
        #endregion;

        private IServiceProvider InternalServiceProvider
        {
            get
            {
                CheckDisposed();

                if (_serviceScope == null)
                {
                    _serviceScope = ServiceProviderCache.Instance.GetOrAdd(_options)
                        .GetRequiredService<IServiceScopeFactory>()
                        .CreateScope();
                }

                return _serviceScope.ServiceProvider;
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
