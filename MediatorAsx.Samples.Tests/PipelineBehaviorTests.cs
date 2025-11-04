using MediatorAsx;
using MediatorAsx.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MediatorAsx.Samples.Tests;

public class PipelineBehaviorTests
{
    [Fact]
    public async Task SendAsync_ExecutesPipelineBehaviorsAroundHandler()
    {
        var services = new ServiceCollection();
        services.AddSingleton<PipelineTrackingContext>();
        services.AddMediator(typeof(PipelineBehaviorTests).Assembly);

        var provider = services.BuildServiceProvider();
        var mediator = provider.GetRequiredService<IMediator>();
        var context = provider.GetRequiredService<PipelineTrackingContext>();

        var response = await mediator.SendAsync(new TrackingRequest("payload"));

        Assert.Equal("handled-payload", response);
        Assert.Equal(
            new[]
            {
                "first-before",
                "second-before",
                "handler",
                "second-after",
                "first-after"
            },
            context.Steps);
    }
}

public sealed record TrackingRequest(string Value) : IRequest<string>;

public sealed class TrackingRequestHandler(PipelineTrackingContext context) : IHandler<TrackingRequest, string>
{
    public Task<string> HandleAsync(TrackingRequest request, CancellationToken cancellationToken = default)
    {
        context.Steps.Add("handler");
        return Task.FromResult($"handled-{request.Value}");
    }
}

public sealed class FirstPipelineBehavior(PipelineTrackingContext context) : IPipelineBehavior<TrackingRequest, string>
{
    public async Task<string> HandleAsync(TrackingRequest request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
    {
        context.Steps.Add("first-before");
        var response = await next();
        context.Steps.Add("first-after");
        return response;
    }
}

public sealed class SecondPipelineBehavior(PipelineTrackingContext context) : IPipelineBehavior<TrackingRequest, string>
{
    public async Task<string> HandleAsync(TrackingRequest request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken)
    {
        context.Steps.Add("second-before");
        var response = await next();
        context.Steps.Add("second-after");
        return response;
    }
}

public sealed class PipelineTrackingContext
{
    public List<string> Steps { get; } = [];
}
