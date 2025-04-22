📋 Índice
Características

Frameworks Suportados

Instalação

Como Usar

Registro de Serviços

Implementação de Requests e Handlers

Usando o Mediator

Exemplos Avançados

Licença

✨ Características
Implementação leve e eficiente do padrão Mediator

Suporte completo para injeção de dependências

Registro automático de handlers

Suporte a multi-targeting (.NET 8.0 e .NET 9.0)

Zero dependências externas além das bibliotecas padrão do .NET

🛠️ Frameworks Suportados
.NET 8.0

.NET 9.0

📦 Instalação
Via Package Manager Console
Install-Package MediatorAsx

<PackageReference Include="MediatorAsx" Version="2.0.0" />

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

📄 Licença
Este projeto está licenciado sob a Licença MIT.

Desenvolvido por Adriano Severino
