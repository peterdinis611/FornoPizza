using Forno.Contracts;
using Forno.Data;

namespace Forno.Mapping;

public static class OrderTicketMapper
{
    public static OrderTicketMessage ToTicket(OvenOrder order, string eventName) =>
        new(
            order.Id,
            order.CreatedAt,
            eventName,
            order.Name,
            order.Phone,
            order.Address,
            order.Note,
            order.Fulfillment,
            order.Status,
            order.Total,
            order.Lines.Select(line => new OrderTicketLine(
                line.PizzaSlug,
                line.PizzaName,
                line.Quantity,
                line.UnitPrice,
                line.Extras)).ToList());
}
