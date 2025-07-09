# MediatorAsx

MediatorAsx é uma implementação simples e leve do padrão Mediator para aplicações .NET, focada em desacoplamento e organização do código através de requisições e handlers.

## Requisitos

- .NET SDK (versões 8.0 ou 9.0)

## Como gerar o pacote NuGet

Para gerar os pacotes NuGet (.nupkg) das bibliotecas `MediatorAsx` e `MediatorAsx.Abstractions`, navegue até o diretório raiz do projeto (`C:/Users/Adria/source/repos/Adriano-Severino/MediatorAsx/`) e execute o seguinte comando:

```bash
dotnet pack --configuration Release
```

Os pacotes gerados estarão localizados nas pastas `nupkgs` dentro dos diretórios de cada projeto (`MediatorAsx/nupkgs` e `MediatorAsx.Abstractions/nupkgs`).

## Como usar

### 1. Instalação

Adicione os pacotes `MediatorAsx` e `MediatorAsx.Abstractions` ao seu projeto via NuGet.

```bash
dotnet add package MediatorAsx
dotnet add package MediatorAsx.Abstractions
```

### 2. Configuração da Injeção de Dependência

No seu `Program.cs` ou na classe de inicialização da sua aplicação, configure o Mediator e registre seus handlers:

```csharp
using MediatorAsx;
using MediatorAsx.Abstractions;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// Adiciona o Mediator e registra todos os handlers (IRequest, INotification) no assembly especificado.
// Você pode passar typeof(Startup).Assembly ou typeof(Program).Assembly, dependendo da sua estrutura.
services.AddMediator(typeof(Program).Assembly);

// Exemplo de registro de outras dependências
services.AddSingleton<IProductRepository, ProductRepository>();

var servicesProvider = services.BuildServiceProvider();
var mediator = servicesProvider.GetRequiredService<IMediator>();

// ... (veja o exemplo de uso abaixo)
```

### 3. Definindo Requisições e Handlers

Crie suas classes de requisição (que implementam `IRequest<TResponse>`) e seus handlers correspondentes (que implementam `IHandler<TRequest, TResponse>`):

```csharp
// Requisição
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

// Exemplo de modelo de dados (Product.cs)
public class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// Exemplo de repositório (IProductRepository.cs e ProductRepository.cs)
public interface IProductRepository
{
    Task<Product> Add(CreateProductCommand product);
}

public class ProductRepository : IProductRepository
{
    public Task<Product> Add(CreateProductCommand product)
    {
        // Simula a adição ao banco de dados
        return Task.FromResult(new Product { Name = product.Name, Price = product.Price });
    }
}
```

### 4. Enviando Requisições

Use a instância de `IMediator` para enviar suas requisições:

```csharp
var request = new CreateProductCommand
{
    Name = "Sample Product",
    Price = 10.00m
};

var result = await mediator.SendAsync(request);

Console.WriteLine($"New product created: {result.Name} with price: {result.Price}");
```

## Licença

Este projeto está licenciado sob a Licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.
