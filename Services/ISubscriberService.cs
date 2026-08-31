using Forno.Contracts;

namespace Forno.Services;

public interface ISubscriberService
{
    Task<Result> AddAsync(string email, CancellationToken cancellation = default);
}
