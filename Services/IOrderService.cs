using Forno.Contracts;

namespace Forno.Services;

public interface IOrderService
{
    Task<Result<OrderReceipt>> PlaceAsync(PlaceOrderRequest request, CancellationToken cancellation = default);
}
