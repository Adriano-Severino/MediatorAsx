# MediatorAsx.Abstractions

Interfaces base do MediatorAsx: IRequest<T>, IHandler<TReq, TRes>, INotification e INotificationHandler<T>.

- Multi-target: .NET 8 e .NET 9
- Licença: MIT

Uso típico:

```csharp
public interface IRequest<TResponse> { }

public interface IHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}

public interface INotification { }

public interface INotificationHandler<TNotification>
    where TNotification : INotification
{
    Task HandleAsync(TNotification notification, CancellationToken cancellationToken = default);
}
```
