using Microsoft.Extensions.Logging;
using System;

namespace HEF.Data
{
    public class DbContextOptionsBuilder : IDbContextOptionsBuilder
    {
        private DbContextOptions _options;

        public DbContextOptionsBuilder()
            : this(new DbContextOptions())
        { }

        public DbContextOptionsBuilder(DbContextOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public virtual DbContextOptions Options => _options;

        public void AddOrUpdateModule<TModule>(TModule module)
            where TModule : class, IDbContextOptionsModule
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            _options = _options.WithModule(module);
        }

        public virtual DbContextOptionsBuilder UseInternalServiceProvider(IServiceProvider serviceProvider)
            => WithOption(e => e.WithInternalServiceProvider(serviceProvider));

        public virtual DbContextOptionsBuilder UseApplicationServiceProvider(IServiceProvider serviceProvider)
            => WithOption(e => e.WithApplicationServiceProvider(serviceProvider));

        public virtual DbContextOptionsBuilder UseLoggerFactory(ILoggerFactory loggerFactory)
            => WithOption(e => e.WithLoggerFactory(loggerFactory));

        private DbContextOptionsBuilder WithOption(Func<CoreOptionsModule, CoreOptionsModule> withFunc)
        {
            ((IDbContextOptionsBuilder)this).AddOrUpdateModule(
                withFunc(Options.FindModule<CoreOptionsModule>() ?? new CoreOptionsModule()));

            return this;
        }
    }
}
