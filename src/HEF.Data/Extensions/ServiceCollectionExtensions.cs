using HEF.Data;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDbContext<TContext>(
            this IServiceCollection serviceCollection,
            Action<DbContextOptionsBuilder> optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TContext : DbContext
            => AddDbContext<TContext, TContext>(serviceCollection, optionsAction, contextLifetime, optionsLifetime);

        public static IServiceCollection AddDbContext<TContextService, TContextImplementation>(
            this IServiceCollection serviceCollection,
            Action<DbContextOptionsBuilder> optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TContextImplementation : DbContext, TContextService
            => AddDbContext<TContextService, TContextImplementation>(
                serviceCollection,
                optionsAction == null
                    ? (Action<IServiceProvider, DbContextOptionsBuilder>)null
                    : (p, b) => optionsAction.Invoke(b),
                contextLifetime, optionsLifetime);

        public static IServiceCollection AddDbContext<TContextService, TContextImplementation>(
            this IServiceCollection serviceCollection,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Scoped,
            ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TContextImplementation : DbContext, TContextService
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            if (contextLifetime == ServiceLifetime.Singleton)
            {
                optionsLifetime = ServiceLifetime.Singleton;
            }

            serviceCollection.AddLogging();

            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(DbContextOptions),
                    p => DbContextOptionsFactory(p, optionsAction),
                    optionsLifetime));

            serviceCollection.TryAdd(new ServiceDescriptor(typeof(TContextService), typeof(TContextImplementation), contextLifetime));

            return serviceCollection;
        }

        private static DbContextOptions DbContextOptionsFactory(
            IServiceProvider applicationServiceProvider,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction)            
        {
            var builder = new DbContextOptionsBuilder(
                new DbContextOptions(new Dictionary<Type, IDbContextOptionsModule>()));

            builder.UseApplicationServiceProvider(applicationServiceProvider);

            optionsAction?.Invoke(applicationServiceProvider, builder);

            return builder.Options;
        }
    }
}
