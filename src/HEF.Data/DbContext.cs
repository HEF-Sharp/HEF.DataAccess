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
        private readonly Lazy<IDbConnection> _connection;

        private IServiceScope _serviceScope;
        private IDbConnectionProvider _connectionProvider;        

        private bool _disposed;

        public DbContext(DbContextOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));

            _connection = new Lazy<IDbConnection>(() => ConnectionProvider.Connection);
        }

        #region Connection
        public IDbConnection Connection => _connection.Value;

        protected virtual IDbConnectionProvider ConnectionProvider
        {
            get
            {
                CheckDisposed();

                return _connectionProvider ?? (_connectionProvider = InternalServiceProvider.GetRequiredService<IDbConnectionProvider>());
            }
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
                _connectionProvider?.Dispose();
            }
        }
        #endregion
    }
}
