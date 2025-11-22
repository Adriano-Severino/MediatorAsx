# MediatorAsx

MediatorAsx is a lightweight, no-nonsense implementation of the Mediator pattern for .NET applications. It focuses on decoupling and clean code organization through requests and handlers.

## Latest updates

- **Pipeline behaviors** (`IPipelineBehavior<TRequest, TResponse>`): wrap handler execution with cross-cutting concerns such as logging, validation, metrics, caching, etc.
- **Sample test project** (`MediatorAsx.Samples.Tests`): covers request sending, notification publishing, and pipeline chaining scenarios.
- **.NET 10 compatibility**: libraries and sample projects now target .NET 10 in addition to the previous frameworks.

## Requirements

- .NET SDK (version 8.0, 9.0, or 10.0)

## Building the NuGet packages

To generate the NuGet packages (`.nupkg`) for `MediatorAsx` and `MediatorAsx.Abstractions`, go to the repo root (`C:/Users/Adria/source/repos/Adriano-Severino/MediatorAsx/`) and run:

```bash
dotnet pack --configuration Release
```

The resulting packages will be placed inside each project’s `nupkgs` folder (`MediatorAsx/nupkgs` and `MediatorAsx.Abstractions/nupkgs`).

## Publishing to NuGet.org (GitHub Actions)

This repository ships with a ready-to-go workflow (`.github/workflows/publish-nuget.yml`) that restores, packs, and publishes both packages to nuget.org.

1) Configure the secret
- GitHub Settings > Secrets and variables > Actions > New repository secret
- Name: `NUGET_API_KEY`
- Value: your nuget.org API key (Push scope). Never commit the key to the repo.

2) Trigger the workflow
- Push a tag that starts with `v` (e.g., `v2.1.0`), or
- Create a GitHub Release with that tag, or
- Run the workflow manually (Actions > publish-nuget > Run workflow).

The workflow publishes both packages:
- `MediatorAsx`
- `MediatorAsx.Abstractions`

3) Versioning checklist
- Before tagging, update the `<Version>` property in:
  - `MediatorAsx/MediatorAsx.csproj`
  - `MediatorAsx.Abstractions/MediatorAsx.Abstractions.csproj`
- Use unpublished version numbers on nuget.org to avoid conflicts.

4) Security best practices
- Store your API key exclusively in GitHub Secrets.
- Never paste keys into issues, commits, PRs, or logs.


## How to use

### 1. Installation

Add both packages to your project via NuGet:

```bash
dotnet add package MediatorAsx
dotnet add package MediatorAsx.Abstractions
```

### 2. Dependency Injection setup

Configure the mediator and register your handlers in `Program.cs` (or your startup entry point):

```csharp
using MediatorAsx;
using MediatorAsx.Abstractions;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// Adds the mediator and registers IRequest/INotification handlers from the provided assembly.
// Plug in typeof(Startup).Assembly, typeof(Program).Assembly, or any assembly you want to scan.
services.AddMediator(typeof(Program).Assembly);

// Example: register additional services
services.AddSingleton<IProductRepository, ProductRepository>();

var serviceProvider = services.BuildServiceProvider();
var mediator = serviceProvider.GetRequiredService<IMediator>();

// ...continue with your app flow
```

### 3. Requests and handlers

Define your requests (`IRequest<TResponse>`) and their respective handlers (`IHandler<TRequest, TResponse>`):

```csharp
// Request
public class CreateProductCommand : IRequest<Product>
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// Handler
public class CreateProductCommandHandler(IProductRepository productRepository) : IHandler<CreateProductCommand, Product>
{
    public async Task<Product> HandleAsync(CreateProductCommand request, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Creating product: {request.Name} with price: {request.Price}");
        var result = await productRepository.Add(request);
        return result;
    }
}

// Data model sample (Product.cs)
public class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// Repository sample (IProductRepository.cs and ProductRepository.cs)
public interface IProductRepository
{
    Task<Product> Add(CreateProductCommand product);
}

public class ProductRepository : IProductRepository
{
    public Task<Product> Add(CreateProductCommand product)
    {
        // Simulate persistence
        return Task.FromResult(new Product { Name = product.Name, Price = product.Price });
    }
}
```

### 4. Sending requests

Use `IMediator` to send commands/queries:

```csharp
var request = new CreateProductCommand
{
    Name = "Sample Product",
    Price = 10.00m
};

var result = await mediator.SendAsync(request);

Console.WriteLine($"New product created: {result.Name} with price: {result.Price}");
```

### 5. Adding pipeline behaviors

Implement `IPipelineBehavior<TRequest, TResponse>` to execute logic before/after the handler. Behaviors are resolved and chained in registration order (the last registered behavior runs closest to the handler).

```csharp
public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {Request}", typeof(TRequest).Name);
        var response = await next();
        logger.LogInformation("Handled {Request}", typeof(TRequest).Name);
        return response;
    }
}

// Registered automatically via AddMediator as long as the behavior lives in a scanned assembly.
```

`MediatorAsx.Samples.Tests/PipelineBehaviorTests.cs` showcases multiple behaviors chained together.

## Tests

Run every scenario (samples included) with:

```bash
dotnet test
```

`MediatorAsx.Samples.Tests` ensures both the default flows (`SendAsync`, `PublishAsync`) and the pipeline composition keep working.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.
