using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace HEF.Data.Internal
{
    public class ServiceProviderCache
    {
        private readonly ConcurrentDictionary<long, IServiceProvider> _serviceProviderDict
            = new ConcurrentDictionary<long, IServiceProvider>();

        public static ServiceProviderCache Instance { get; } = new ServiceProviderCache();

        public virtual IServiceProvider GetOrAdd(IDbContextOptions options)
        {
            var key = options.Modules
                .OrderBy(e => e.GetType().Name)
                .Aggregate(0L, (t, e) => (t * 397) ^ ((long)e.GetType().GetHashCode() * 397) ^ e.GetServiceProviderHashCode());

            var internalServiceProvider = options.FindModule<CoreOptionsModule>()?.InternalServiceProvider;
            if (internalServiceProvider != null)
            {
                return internalServiceProvider;
            }

            return _serviceProviderDict.GetOrAdd(key, k =>
            {
                var services = new ServiceCollection();
                var hasProvider = ApplyServices(options, services);

                return services.BuildServiceProvider();
            });
        }

        private static bool ApplyServices(IDbContextOptions options, ServiceCollection services)
        {
            var providerAdded = false;

            foreach (var module in options.Modules)
            {
                if (module.ApplyServices(services))
                {
                    providerAdded = true;
                }
            }

            return providerAdded;
        }
    }
}
