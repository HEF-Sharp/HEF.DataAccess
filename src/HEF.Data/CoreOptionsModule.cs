using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace HEF.Data
{
    public class CoreOptionsModule : IDbContextOptionsModule
    {
        private IServiceProvider _applicationServiceProvider;
        private ILoggerFactory _loggerFactory;

        public CoreOptionsModule()
        { }

        protected CoreOptionsModule(CoreOptionsModule copyFrom)
        {
            if (copyFrom == null)
                throw new ArgumentNullException(nameof(copyFrom));

            _applicationServiceProvider = copyFrom.ApplicationServiceProvider;
            _loggerFactory = copyFrom.LoggerFactory;
        }

        protected virtual CoreOptionsModule Clone() => new CoreOptionsModule(this);

        public virtual IServiceProvider ApplicationServiceProvider => _applicationServiceProvider;

        public virtual ILoggerFactory LoggerFactory => _loggerFactory;

        public virtual CoreOptionsModule WithApplicationServiceProvider(IServiceProvider applicationServiceProvider)
        {
            var clone = Clone();

            clone._applicationServiceProvider = applicationServiceProvider;

            return clone;
        }

        public virtual CoreOptionsModule WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            var clone = Clone();

            clone._loggerFactory = loggerFactory;

            return clone;
        }

        public virtual bool ApplyServices(IServiceCollection services)
        {
            var loggerFactory = GetLoggerFactory();
            if (loggerFactory != null)
            {
                services.AddSingleton(loggerFactory);
            }

            return false;  //since no database provider is registered
        }

        private ILoggerFactory GetLoggerFactory()
            => LoggerFactory ?? ApplicationServiceProvider?.GetService<ILoggerFactory>();
    }
}
