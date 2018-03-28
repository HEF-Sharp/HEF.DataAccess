using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HEF.Data.Storage
{
    public class NamedConnectionStringResolver : INamedConnectionStringResolver
    {
        private const string DefaultSection = "ConnectionStrings:";

        private readonly IDbContextOptions _options;

        public NamedConnectionStringResolver(IDbContextOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        protected virtual IServiceProvider ApplicationServiceProvider
            => _options.FindModule<CoreOptionsModule>()
                ?.ApplicationServiceProvider;

        public virtual string ResolveConnectionString(string connectionString)
        {
            var connectionName = TryGetConnectionName(connectionString);

            if (connectionName == null)
                return connectionString;

            var configuration = ApplicationServiceProvider
                ?.GetService<IConfiguration>();

            var resolved = configuration?[connectionName]
                           ?? configuration?[DefaultSection + connectionName];

            if (resolved == null)            
                throw new InvalidOperationException(
                    $"the named '{connectionName}' connection string was not found in the application's configuration");            

            return resolved;
        }

        private static string TryGetConnectionName(string connectionString)
        {
            var firstEquals = connectionString.IndexOf('=');
            if (firstEquals < 0)
                return null;

            if (connectionString.IndexOf('=', firstEquals + 1) >= 0)
                return null;

            if (connectionString.Substring(0, firstEquals).Trim().Equals(
                "name", StringComparison.OrdinalIgnoreCase))
            {
                return connectionString.Substring(firstEquals + 1).Trim();
            }

            return null;
        }
    }
}
