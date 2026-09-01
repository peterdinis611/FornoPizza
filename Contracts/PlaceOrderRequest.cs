using Forno.Domain;
using Forno.Models;

namespace Forno.Contracts;

public sealed record PlaceOrderRequest(
    string Name,
    string Phone,
    string Address,
    string Note,
    IReadOnlyList<CartLine> Lines,
    FulfillmentMode Fulfillment = FulfillmentMode.Delivery);
