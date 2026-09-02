namespace Forno.Contracts;

public sealed record AdminOrderLine(
    string PizzaSlug,
    string PizzaName,
    int Quantity,
    decimal UnitPrice,
    string Extras,
    decimal LineTotal);

public sealed record AdminOrder(
    int Id,
    DateTimeOffset CreatedAt,
    string Name,
    string Email,
    string Phone,
    string Address,
    string Note,
    string Fulfillment,
    string Status,
    decimal Total,
    string? StripeSessionId,
    IReadOnlyList<AdminOrderLine> Lines);

public sealed record AdminStats(
    int TodayCount,
    decimal TodayRevenue,
    int KitchenCount,
    int PendingPaymentCount,
    int AllCount);

public sealed record AdminOrderPage(
    IReadOnlyList<AdminOrder> Items,
    AdminStats Stats);
