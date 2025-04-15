using MediatorAsx;
using Microsoft.Extensions.DependencyInjection;

namespace MyMediator.Application
{
    public static class DependencyInjecttion
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediator(typeof(DependencyInjecttion).Assembly);
            return services;
        }
    }
}
