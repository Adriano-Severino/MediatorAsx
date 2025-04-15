using MediatorAsx;
using MediatorAsx.Abstractions;
using MediatorAsx.Samples;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddMediator(typeof(Program).Assembly);
services.AddSingleton<IProductRepository, ProductRepository>();

var servicesProvider = services.BuildServiceProvider();
var mediator = servicesProvider.GetRequiredService<IMediator>();

var request = new CreateProductCommand
{
    Name = "Sample Product",
    Price = 10.00m
};

var result = await mediator.SendAsync(request);

Console.WriteLine($"New product created: {result.Name} with price: {result.Price}");
public class CreateProductCommand : IRequest<Product>
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}
public class CreateProductCommandHandler(IProductRepository productRepository) : IHandler<CreateProductCommand, Product>
{
    public async Task<Product> HandleAsync(CreateProductCommand request, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Creating product: {request.Name} with price: {request.Price}");
        var result = await productRepository.Add(request);

        return result;
    }
}