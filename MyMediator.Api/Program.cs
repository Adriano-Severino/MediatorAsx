using MediatorAsx.Abstractions;
using MyMediator.Application;
using MyMediator.Application.Acconts.Repositories.Abstractions;
using MyMediator.Infrastructure.Accounts.Repositories;
using CreatedAtActionResult = MyMediator.Application.Acconts.UseCases.Create.Request;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IAccountRepository, AccountRepository>();
builder.Services.AddApplication();

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapPost("/v1/accounts", async (IMediator mediator, CreatedAtActionResult command) =>
{
    var result = await mediator.SendAsync(command);
    return Results.Ok(result);
});


app.Run();