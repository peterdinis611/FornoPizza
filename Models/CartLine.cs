namespace Forno.Models;

public sealed class CartLine
{
    public required PizzaItem Pizza { get; init; }
    public int Quantity { get; set; } = 1;
    public decimal LineTotal => Pizza.Price * Quantity;
}
