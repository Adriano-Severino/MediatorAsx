using MediatorAsx.Abstractions;

namespace MyMediator.Application.Acconts.UseCases.Create
{
    public class Request : IRequest<string>
    {
        public string Name { get; set; } = string.Empty;
    }
}
