using MediatorAsx.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MediatorAsx
{
    public static class MediatorExtension
    {
        public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddTransient<IMediator, Mediator>();

            var handlerTypes = typeof(IHandler<,>);

            foreach (var assembly in assemblies)
            {
                var handlers = assembly.GetTypes()
                    .Where(type => !type.IsAbstract && !type.IsInterface)
                    .SelectMany(x => x.GetInterfaces(), (t, i) => new { type = t, Interface = i })
                    .Where(ti => ti.Interface.IsGenericType && ti.Interface.GetGenericTypeDefinition() == handlerTypes);

                foreach (var handler in handlers)
                {
                    services.AddTransient(handler.Interface, handler.type);
                }
            }


            return services;
        }
    }
}
