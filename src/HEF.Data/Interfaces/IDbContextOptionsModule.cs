using Microsoft.Extensions.DependencyInjection;

namespace HEF.Data
{
    public interface IDbContextOptionsModule
    {
        bool ApplyServices(IServiceCollection services);

        long GetServiceProviderHashCode();
    }
}
