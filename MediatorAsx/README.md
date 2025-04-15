# MediatorAsx

Uma implementação simples e eficiente do padrão Mediator para .NET.

## Como usar

### Registro de Serviços
services.AddMediator(typeof(Startup).Assembly);


### Implementação de Requests e Handlers

public class ExemploRequest : IRequest<string>
{
public string Valor { get; set; }
}

public class ExemploHandler : IHandler<ExemploRequest, string>
{
public Task<string> HandleAsync(ExemploRequest request, CancellationToken cancellationToken = default)
{
return Task.FromResult($"Resultado: {request.Valor}");
}
}


### Usando o Mediator
Public class MinhaClasse
{
private readonly IMediator _mediator;
public MinhaClasse(IMediator mediator)
{
    _mediator = mediator;
}

public async Task<string> ExecutarAsync()
{
    var request = new ExemploRequest { Valor = "Teste" };
    return await _mediator.SendAsync(request);
}
