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

            // Tipos de handlers
            var handlerTypes = typeof(IHandler<,>);
            var notificationHandlerTypes = typeof(INotificationHandler<>);
            var pipelineBehaviorTypes = typeof(IPipelineBehavior<,>);

            foreach (var assembly in assemblies)
            {
                // Registrar handlers de requisição
                var handlers = assembly.GetTypes()
                    .Where(type => !type.IsAbstract && !type.IsInterface)
                    .SelectMany(x => x.GetInterfaces(), (t, i) => new { type = t, Interface = i })
                    .Where(ti => ti.Interface.IsGenericType && ti.Interface.GetGenericTypeDefinition() == handlerTypes);

                foreach (var handler in handlers)
                {
                    services.AddTransient(handler.Interface, handler.type);
                }

                // Registrar handlers de notificação
                var notificationHandlers = assembly.GetTypes()
                    .Where(type => !type.IsAbstract && !type.IsInterface)
                    .SelectMany(x => x.GetInterfaces(), (t, i) => new { type = t, Interface = i })
                    .Where(ti => ti.Interface.IsGenericType && ti.Interface.GetGenericTypeDefinition() == notificationHandlerTypes);

                foreach (var handler in notificationHandlers)
                {
                    services.AddTransient(handler.Interface, handler.type);
                }

                // Registrar pipeline behaviors
                var pipelineBehaviors = assembly.GetTypes()
                    .Where(type => !type.IsAbstract && !type.IsInterface)
                    .SelectMany(x => x.GetInterfaces(), (t, i) => new { type = t, Interface = i })
                    .Where(ti => ti.Interface.IsGenericType && ti.Interface.GetGenericTypeDefinition() == pipelineBehaviorTypes);

                foreach (var behavior in pipelineBehaviors)
                {
                    services.AddTransient(behavior.Interface, behavior.type);
                }
            }

            return services;
        }
    }
}
