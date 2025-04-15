
using MediatorAsx.Abstractions;
using MyMediator.Application.Acconts.Repositories.Abstractions;
using MyMediator.Domain;

namespace MyMediator.Application.Acconts.UseCases.Create
{
    public class Handler(IAccountRepository accountRepository) : IHandler<Request, string>
    {
        public async Task<string> HandleAsync(Request request, CancellationToken cancellationToken = default)
        {
            var account = new Account
            {
                Id = 0,
                Name = request.Name,
            };

            await accountRepository.SaveAsync(account, cancellationToken);

            return $"Account {account.Id} - {account.Name} has been saved.";
        }
    }
}
