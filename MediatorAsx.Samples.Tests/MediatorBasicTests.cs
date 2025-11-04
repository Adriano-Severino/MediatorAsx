using MediatorAsx;
using MediatorAsx.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MediatorAsx.Samples.Tests;

public class MediatorBasicTests
{
    [Fact]
    public async Task SendAsync_ResolvesRegisteredHandler()
    {
        var services = new ServiceCollection();
        services.AddSingleton<HandlerTracker>();
        services.AddMediator(typeof(MediatorBasicTests).Assembly);

        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();
        var tracker = provider.GetRequiredService<HandlerTracker>();

        var response = await mediator.SendAsync(new EchoRequest("value"));

        Assert.Equal("value", response);
        Assert.True(tracker.HandlerInvoked);
    }

    [Fact]
    public async Task PublishAsync_InvokesAllNotificationHandlers()
    {
        var services = new ServiceCollection();
        services.AddSingleton<HandlerTracker>();
        services.AddMediator(typeof(MediatorBasicTests).Assembly);

        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();
        var tracker = provider.GetRequiredService<HandlerTracker>();

        await mediator.PublishAsync(new SampleNotification("payload"));

        Assert.Equal(new[] { "first", "second" }, tracker.NotificationOrder);
    }
}

public sealed record EchoRequest(string Value) : IRequest<string>;

public sealed class EchoRequestHandler(HandlerTracker tracker) : IHandler<EchoRequest, string>
{
    public Task<string> HandleAsync(EchoRequest request, CancellationToken cancellationToken = default)
    {
        tracker.HandlerInvoked = true;
        return Task.FromResult(request.Value);
    }
}

public sealed record SampleNotification(string Value) : INotification;

public sealed class FirstNotificationHandler(HandlerTracker tracker) : INotificationHandler<SampleNotification>
{
    public Task HandleAsync(SampleNotification notification, CancellationToken cancellationToken = default)
    {
        tracker.NotificationOrder.Add("first");
        return Task.CompletedTask;
    }
}

public sealed class SecondNotificationHandler(HandlerTracker tracker) : INotificationHandler<SampleNotification>
{
    public Task HandleAsync(SampleNotification notification, CancellationToken cancellationToken = default)
    {
        tracker.NotificationOrder.Add("second");
        return Task.CompletedTask;
    }
}

public sealed class HandlerTracker
{
    public bool HandlerInvoked { get; set; }

    public List<string> NotificationOrder { get; } = [];
}
