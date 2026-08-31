namespace Forno.Data;

public sealed class OrderLine
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public OvenOrder Order { get; set; } = null!;

    public int PizzaId { get; set; }
    public string PizzaSlug { get; set; } = "";
    public string PizzaName { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string Extras { get; set; } = "";

    public decimal LineTotal => UnitPrice * Quantity;
}
