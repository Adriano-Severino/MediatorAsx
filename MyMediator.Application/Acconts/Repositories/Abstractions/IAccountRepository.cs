using MyMediator.Domain;

namespace MyMediator.Application.Acconts.Repositories.Abstractions
{
    public interface IAccountRepository
    {
        Task SaveAsync(Account account, CancellationToken cancellationToken = default);
    }
}
