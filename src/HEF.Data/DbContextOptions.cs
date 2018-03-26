using System;
using System.Collections.Generic;
using System.Linq;

namespace HEF.Data
{
    public class DbContextOptions : IDbContextOptions
    {
        private readonly IReadOnlyDictionary<Type, IDbContextOptionsModule> _modules;

        public DbContextOptions()
            : this(new Dictionary<Type, IDbContextOptionsModule>())
        { }

        public DbContextOptions(IReadOnlyDictionary<Type, IDbContextOptionsModule> modules)
        {
            _modules = modules ?? throw new ArgumentNullException(nameof(modules));
        }

        public virtual IEnumerable<IDbContextOptionsModule> Modules => _modules.Values;

        public virtual TModule FindModule<TModule>()
            where TModule : class, IDbContextOptionsModule
            => _modules.TryGetValue(typeof(TModule), out var module) ? (TModule)module : null;

        public virtual DbContextOptions WithModule<TModule>(TModule module)
            where TModule : class, IDbContextOptionsModule
        {
            var modules = Modules.ToDictionary(p => p.GetType(), p => p);
            modules[typeof(TModule)] = module ?? throw new ArgumentNullException(nameof(module));

            return new DbContextOptions(modules);
        }
    }
}
