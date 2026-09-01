using Forno.Contracts;
using Forno.Models;

namespace Forno.Services;

public interface IShopService
{
    Task<PizzaItem?> DaySpecialAsync(CancellationToken cancellation = default);

    Task<Result<string>> SetDaySpecialAsync(string slug, CancellationToken cancellation = default);
}
