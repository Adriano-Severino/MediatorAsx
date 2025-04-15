using MyMediator.Application.Acconts.Repositories.Abstractions;
using MyMediator.Domain;

namespace MyMediator.Infrastructure.Accounts.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public Task SaveAsync(Account account, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"Account {account.Id} has been saved.");
            return Task.CompletedTask;
        }
    }
}
