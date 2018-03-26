using System.Collections.Generic;

namespace HEF.Data
{
    public interface IDbContextOptions
    {
        IEnumerable<IDbContextOptionsModule> Modules { get; }

        TModule FindModule<TModule>()
            where TModule : class, IDbContextOptionsModule;
    }
}
