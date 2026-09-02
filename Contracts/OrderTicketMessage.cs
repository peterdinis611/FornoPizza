namespace Forno.Contracts;

public sealed record OrderTicketLine(
    string PizzaSlug,
    string PizzaName,
    int Quantity,
    decimal UnitPrice,
    string Extras);

public sealed record OrderTicketMessage(
    int OrderId,
    DateTimeOffset CreatedAt,
    string Event,
    string Name,
    string Phone,
    string Address,
    string Note,
    string Fulfillment,
    string Status,
    decimal Total,
    IReadOnlyList<OrderTicketLine> Lines);
