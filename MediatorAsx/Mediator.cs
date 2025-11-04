using MediatorAsx.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace MediatorAsx
{
    public class Mediator(IServiceProvider serviceProvider) : IMediator
    {
        public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var requestType = request.GetType();
            var handlerType = typeof(IHandler<,>).MakeGenericType(requestType, typeof(TResponse));

            var handler = serviceProvider.GetService(handlerType);

            if (handler is null)
            {
                throw new InvalidOperationException($"Handler not found for {requestType}");
            }

            var handlerMethod = handlerType.GetMethod("HandleAsync");
            if (handlerMethod is null)
                throw new InvalidOperationException($"Method not found for {handlerType}");

            RequestHandlerDelegate<TResponse> handlerDelegate = () =>
            {
                var handlerResult = handlerMethod.Invoke(handler, new object[] { request, cancellationToken });
                if (handlerResult is not Task<TResponse> handlerTask)
                    throw new InvalidOperationException($"Handler returned unexpected type {handlerResult}");

                return handlerTask;
            };

            var pipelineType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));
            var pipelineInstances = serviceProvider.GetServices(pipelineType).Cast<object>().ToArray();

            if (pipelineInstances.Length is 0)
            {
                return await handlerDelegate();
            }

            var pipelineMethod = pipelineType.GetMethod("HandleAsync");
            if (pipelineMethod is null)
                throw new InvalidOperationException($"Pipeline method not found for {pipelineType}");

            RequestHandlerDelegate<TResponse> next = handlerDelegate;

            foreach (var pipeline in pipelineInstances.Reverse())
            {
                var currentNext = next;
                next = () =>
                {
                    var pipelineResult = pipelineMethod.Invoke(pipeline, new object[] { request, currentNext, cancellationToken });
                    if (pipelineResult is not Task<TResponse> pipelineTask)
                        throw new InvalidOperationException($"Pipeline behavior {pipeline.GetType().Name} returned unexpected type {pipelineResult}");

                    return pipelineTask;
                };
            }

            return await next();
        }

        public async Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
             where TNotification : INotification
        {
            var notificationType = notification.GetType();
            var handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);

            // Obtém todos os handlers registrados para este tipo de notificação
            var handlers = serviceProvider.GetServices(handlerType);

            if (!handlers.Any())
            {
                // Não há handlers registrados para este tipo de notificação
                return;
            }

            var method = handlerType.GetMethod("HandleAsync");
            if (method is null)
                throw new InvalidOperationException($"Method not found for {handlerType}");

            var tasks = new List<Task>();

            foreach (var handler in handlers)
            {
                var result = method.Invoke(handler, [notification, cancellationToken]);
                if (result is Task task)
                {
                    tasks.Add(task);
                }
                else
                {
                    throw new InvalidOperationException($"Handler {handler.GetType().Name} returned a non-Task result");
                }
            }

            await Task.WhenAll(tasks);
        }

    }
}
