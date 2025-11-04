using System.Threading;
using System.Threading.Tasks;

namespace MediatorAsx.Abstractions
{
    /// <summary>
    /// Represents the continuation delegate to invoke the next component in the pipeline.
    /// </summary>
    /// <typeparam name="TResponse">Response type.</typeparam>
    /// <returns>An awaitable task that yields <typeparamref name="TResponse"/>.</returns>
    public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

    /// <summary>
    /// Allows decorating request execution with additional behavior before or after the handler.
    /// </summary>
    /// <typeparam name="TRequest">Request type.</typeparam>
    /// <typeparam name="TResponse">Response type.</typeparam>
    public interface IPipelineBehavior<in TRequest, TResponse> where TRequest : notnull
    {
        /// <summary>
        /// Implement custom logic and optionally invoke the <paramref name="next"/> delegate.
        /// </summary>
        /// <param name="request">Incoming request instance.</param>
        /// <param name="next">Continuation for the next pipeline step or the handler.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Awaitable task that yields <typeparamref name="TResponse"/>.</returns>
        Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
    }
}
