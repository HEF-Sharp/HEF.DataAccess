using HEF.Data.Storage;
using System;
using System.Linq;

namespace HEF.Data
{
    public static class DbContextOptionsExtensions
    {
        public static DatabaseOptionsModule FindDatabaseOptions(this IDbContextOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return options.Modules.OfType<DatabaseOptionsModule>().Single();
        }
    }
}
