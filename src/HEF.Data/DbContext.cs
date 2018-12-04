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

        protected DbContext()
            : this(new DbContextOptions())
        { }

        public DbContext(DbContextOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));

            _connection = new Lazy<IDbConnection>(() => ConnectionProvider.Connection);
        }

        /// <summary>
        ///     <para>
        ///         Override this method to configure the database (and other options) to be used for this context.
        ///         This method is called for each instance of the context that is created.
        ///         The base implementation does nothing.
        ///     </para>
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected internal virtual void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
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
                    var optionsBuilder = new DbContextOptionsBuilder(_options);
                    OnConfiguring(optionsBuilder);

                    _serviceScope = ServiceProviderCache.Instance.GetOrAdd(optionsBuilder.Options)
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
