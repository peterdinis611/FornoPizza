namespace Forno.Models;

public sealed class CartLine
{
    public required PizzaItem Pizza { get; init; }
    public int Quantity { get; set; } = 1;
    public IReadOnlyList<string> ExtraIds { get; init; } = [];

    public decimal ExtraTotal => OvenExtras.Sum(ExtraIds);

    public decimal UnitTotal => Pizza.Price + ExtraTotal;

    public decimal LineTotal => UnitTotal * Quantity;

    public string ExtraLabel => OvenExtras.Label(ExtraIds);

    public string Key =>
        ExtraIds.Count == 0
            ? Pizza.Slug
            : $"{Pizza.Slug}|{string.Join(",", OvenExtras.Normalize(ExtraIds))}";
}
