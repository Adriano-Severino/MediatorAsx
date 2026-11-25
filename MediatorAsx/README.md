📋 Índice
Características

Frameworks Suportados

Instalação

Como Usar

Registro de Serviços

Implementação de Requests e Handlers

Implementação de Notificações e Handlers

Usando o Mediator

Exemplos Avançados

Licença

✨ Características
Implementação leve e eficiente do padrão Mediator

Suporte completo para injeção de dependências

Registro automático de handlers

Suporte a multi-targeting (.NET 8.0, .NET 9.0, .NET 10.0 e .NET 11.0)

Zero dependências externas além das bibliotecas padrão do .NET

🛠️ Frameworks Suportados
.NET 8.0

.NET 9.0

.NET 10.0

.NET 11.0

📦 Instalação
Via Package Manager Console
Install-Package MediatorAsx

<PackageReference Include="MediatorAsx" Version="2.2.0" />

🚀 Como Usar
Registro de Serviços
Registre o mediator no contêiner de injeção de dependências da sua aplicação:

// Em Program.cs ou Startup.cs
using MediatorAsx;

// ...

// Adicione o mediator, passando o assembly onde estão seus handlers
builder.Services.AddMediator(typeof(Program).Assembly);

// Adicione o mediator, passando o assembly onde estão seus handlers
builder.Services.AddMediator(typeof(Program).Assembly);

Implementação de Requests e Handlers
Crie suas classes de request e handler implementando as interfaces IRequest<TResponse> e IHandler<TRequest, TResponse>:

using MediatorAsx.Abstractions;

// Definição do Request
public class ExemploRequest : IRequest<string>
{
    public string Valor { get; set; }
}

// Implementação do Handler
public class ExemploHandler : IHandler<ExemploRequest, string>
{
    public Task<string> HandleAsync(ExemploRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult($"Resultado: {request.Valor}");
    }
}

Usando o Mediator
Injete e utilize o mediator em suas classes:

using MediatorAsx.Abstractions;

public class MinhaClasse
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
}

Implementação de Notificações e Handlers
Crie notificações (eventos) e seus respectivos handlers implementando as interfaces INotification e INotificationHandler<TNotification>:

using MediatorAsx.Abstractions;

// Definição da Notificação
public class ExemploNotification : INotification
{
    public string Mensagem { get; set; }
    public DateTime DataOcorrencia { get; set; } = DateTime.Now;
}

// Implementação do Handler de Notificação
public class ExemploNotificationHandler : INotificationHandler<ExemploNotification>
{
    private readonly ILogger<ExemploNotificationHandler> _logger;
    
    public ExemploNotificationHandler(ILogger<ExemploNotificationHandler> logger)
    {
        _logger = logger;
    }
    
    public Task HandleAsync(ExemploNotification notification, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Notificação recebida: {notification.Mensagem} em {notification.DataOcorrencia}");
        return Task.CompletedTask;
    }
}

Usando o Mediator
Injete e utilize o mediator em suas classes:

using MediatorAsx.Abstractions;

public class MinhaClasse
{
    private readonly IMediator _mediator;
    
    public MinhaClasse(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public async Task<string> ExecutarRequestAsync()
    {
        var request = new ExemploRequest { Valor = "Teste" };
        return await _mediator.SendAsync(request);
    }
    
    public async Task PublicarEventoAsync()
    {
        var notification = new ExemploNotification { 
            Mensagem = "Algo importante aconteceu" 
        };
        
        await _mediator.PublishAsync(notification);
    }
}

📚 Exemplos Avançados
Request com Validação
public class CreateUserRequest : IRequest<UserResult>
{
    public string Nome { get; set; }
    public string Email { get; set; }
    public int Idade { get; set; }
}

public class UserResult
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; }
    public Guid Id { get; set; }
}

public class CreateUserHandler : IHandler<CreateUserRequest, UserResult>
{
    private readonly IUserRepository _repository;
    
    public CreateUserHandler(IUserRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<UserResult> HandleAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        // Validação simples
        if (string.IsNullOrEmpty(request.Nome))
            return new UserResult { Sucesso = false, Mensagem = "Nome é obrigatório" };
            
        if (string.IsNullOrEmpty(request.Email))
            return new UserResult { Sucesso = false, Mensagem = "Email é obrigatório" };
            
        if (request.Idade < 18)
            return new UserResult { Sucesso = false, Mensagem = "Idade mínima é 18 anos" };
        
        // Persistência
        var user = new User 
        {
            Nome = request.Nome,
            Email = request.Email,
            Idade = request.Idade
        };
        
        await _repository.AddAsync(user, cancellationToken);
        
        return new UserResult 
        { 
            Sucesso = true, 
            Mensagem = "Usuário criado com sucesso",
            Id = user.Id
        };
    }
}

Publicação de Eventos de Domínio

// Evento de domínio
public class PedidoCriadoNotification : INotification
{
    public Guid PedidoId { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime DataCriacao { get; set; }
}

// Primeiro handler - Envia email de confirmação
public class EnviarEmailConfirmacaoHandler : INotificationHandler<PedidoCriadoNotification>
{
    private readonly IEmailService _emailService;
    
    public EnviarEmailConfirmacaoHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }
    
    public async Task HandleAsync(PedidoCriadoNotification notification, CancellationToken cancellationToken)
    {
        await _emailService.EnviarEmailConfirmacaoPedidoAsync(
            notification.PedidoId, 
            notification.ValorTotal, 
            cancellationToken);
    }
}

// Segundo handler - Atualiza estatísticas
public class AtualizarEstatisticasHandler : INotificationHandler<PedidoCriadoNotification>
{
    private readonly IEstatisticasService _estatisticasService;
    
    public AtualizarEstatisticasHandler(IEstatisticasService estatisticasService)
    {
        _estatisticasService = estatisticasService;
    }
    
    public async Task HandleAsync(PedidoCriadoNotification notification, CancellationToken cancellationToken)
    {
        await _estatisticasService.RegistrarNovoPedidoAsync(
            notification.ValorTotal, 
            notification.DataCriacao, 
            cancellationToken);
    }
}

// Usando o dispatcher para eventos de domínio
public static class MediatorExtensions
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, BaseEntity entity, CancellationToken cancellationToken = default)
    {
        var domainEvents = entity.DomainEvents.ToList();
        entity.ClearDomainEvents();

        foreach (var domainEvent in domainEvents)
        {
            // Convertendo o evento de domínio para INotification
            if (domainEvent is INotification notification)
            {
                await mediator.PublishAsync(notification, cancellationToken);
            }
        }
    }
}

Considerações Importantes
Ao trabalhar com notificações assíncronas, evite misturar chamadas .Wait() ou .Result com operações assíncronas, pois isso pode causar deadlocks em certas situações. 
Em ambientes onde callbacks ou ações assíncronas são executadas após operações de banco de dados como ToListAsync(), recomenda-se sempre manter o fluxo assíncrono completo.

📄 Licença
Este projeto está licenciado sob a Licença MIT.

Desenvolvido por Adriano Xavier
