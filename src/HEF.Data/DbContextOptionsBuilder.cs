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
    }
}
