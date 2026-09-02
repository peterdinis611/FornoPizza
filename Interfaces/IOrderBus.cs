using Forno.Contracts;
using Forno.Data;

namespace Forno.Interfaces;

public interface IOrderBus
{
    bool IsEnabled { get; }

    Task PublishKitchenTicketAsync(OvenOrder order, string eventName, CancellationToken cancellation = default);
}
