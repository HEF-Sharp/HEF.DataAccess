using HEF.Data.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HEF.Data.MySql.Storage
{
    public class MySqlOptionsModule : DatabaseOptionsModule
    {
        public MySqlOptionsModule()
        { }

        public MySqlOptionsModule(DatabaseOptionsModule copyFrom)
            : base(copyFrom)
        { }

        protected override DatabaseOptionsModule Clone()
            => new MySqlOptionsModule(this);

        public override bool ApplyServices(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddScoped<IDbConnectionProvider, MySqlConnectionProvider>();

            return true;
        }
    }
}
