using Forno.Contracts;
using Forno.Models;

namespace Forno.Interfaces;

public interface IShopService
{
    Task<PizzaItem?> DaySpecialAsync(CancellationToken cancellation = default);

    Task<Result<string>> SetDaySpecialAsync(string slug, CancellationToken cancellation = default);
}
