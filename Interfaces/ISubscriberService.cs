using Forno.Contracts;

namespace Forno.Interfaces;

public interface ISubscriberService
{
    Task<Result> AddAsync(string email, CancellationToken cancellation = default);
}
