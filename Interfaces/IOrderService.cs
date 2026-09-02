using Forno.Contracts;

namespace Forno.Interfaces;

public interface IOrderService
{
    Task<Result<OrderReceipt>> PlaceAsync(PlaceOrderRequest request, CancellationToken cancellation = default);
}
