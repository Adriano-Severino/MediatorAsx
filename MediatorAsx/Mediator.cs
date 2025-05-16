using MediatorAsx.Abstractions;
using Microsoft.Extensions.DependencyInjection;

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

            var method = handlerType.GetMethod("HandleAsync");
            if (method is null)
                throw new InvalidOperationException($"Method not found for {handlerType}");

            var result = method.Invoke(handler, [request, cancellationToken]);
            if (result is not Task<TResponse> task)
                throw new InvalidOperationException($"Invalid returned unexpected type {result}");

            return await task;
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
