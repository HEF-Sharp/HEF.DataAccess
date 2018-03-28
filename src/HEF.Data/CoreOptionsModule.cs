using Microsoft.Extensions.DependencyInjection;
using System;

namespace HEF.Data
{
    public class CoreOptionsModule : IDbContextOptionsModule
    {
        private IServiceProvider _applicationServiceProvider;

        public CoreOptionsModule()
        { }

        protected CoreOptionsModule(CoreOptionsModule copyFrom)
        {
            if (copyFrom == null)
                throw new ArgumentNullException(nameof(copyFrom));

            _applicationServiceProvider = copyFrom.ApplicationServiceProvider;
        }

        protected virtual CoreOptionsModule Clone() => new CoreOptionsModule(this);

        public virtual IServiceProvider ApplicationServiceProvider => _applicationServiceProvider;

        public virtual CoreOptionsModule WithApplicationServiceProvider(IServiceProvider applicationServiceProvider)
        {
            var clone = Clone();

            clone._applicationServiceProvider = applicationServiceProvider;

            return clone;
        }

        public virtual bool ApplyServices(IServiceCollection services)
        {
            return false;  //since no database provider is registered
        }
    }
}
