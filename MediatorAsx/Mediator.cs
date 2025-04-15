using MediatorAsx.Abstractions;

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
    }
}
