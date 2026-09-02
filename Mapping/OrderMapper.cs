using Forno.Contracts;
using Forno.Data;
using Forno.Domain;
using Forno.Models;
using Forno.Validation;

namespace Forno.Mapping;

public static class OrderMapper
{
    public static OrderLine ToLine(CartLine line, Pizza pizza)
    {
        var extras = CartRules.Extras(line.ExtraIds);
        var quantity = CartRules.ClampQty(line.Quantity);
        var unit = pizza.Price + OvenExtras.Sum(extras);

        return new OrderLine
        {
            PizzaId = pizza.Id,
            PizzaSlug = pizza.Slug,
            PizzaName = pizza.Name,
            UnitPrice = unit,
            Quantity = quantity,
            Extras = OvenExtras.Label(extras)
        };
    }

    public static OvenOrder ToOrder(
        PlaceOrderRequest request,
        IReadOnlyList<OrderLine> lines,
        string status = OrderStatus.Accepted) =>
        new()
        {
            CreatedAt = DateTimeOffset.UtcNow,
            Name = NameRules.Normalize(request.Name),
            Email = EmailRules.Normalize(request.Email),
            Phone = PhoneRules.Normalize(request.Phone),
            Address = request.Fulfillment == FulfillmentMode.Pickup
                ? OvenCommerce.PickupAddress
                : AddressRules.Normalize(request.Address),
            Note = InputText.Note(request.Note),
            Fulfillment = request.Fulfillment.ToString().ToLowerInvariant(),
            Status = status,
            Lines = [.. lines],
            Total = lines.Sum(line => line.UnitPrice * line.Quantity)
        };

    public static OrderReceipt ToReceipt(OvenOrder order) =>
        new(order.Id, order.Name, order.Total);
}
