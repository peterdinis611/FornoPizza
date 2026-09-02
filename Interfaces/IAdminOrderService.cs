using Forno.Contracts;

namespace Forno.Interfaces;

public interface IAdminOrderService
{
    Task<AdminStats> StatsAsync(CancellationToken cancellation = default);

    Task<AdminOrderPage> ListAsync(string? status = null, int take = 80, CancellationToken cancellation = default);

    Task<AdminOrder?> GetAsync(int id, CancellationToken cancellation = default);

    Task<Result> SetStatusAsync(int id, string status, CancellationToken cancellation = default);
}
