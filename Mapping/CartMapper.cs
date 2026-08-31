using Forno.Models;
using Forno.Validation;

namespace Forno.Mapping;

public static class CartMapper
{
    public static CartLine ToLine(PizzaItem pizza, int quantity, IEnumerable<string>? extras = null) =>
        new()
        {
            Pizza = pizza,
            Quantity = CartRules.ClampQty(quantity),
            ExtraIds = CartRules.Extras(extras)
        };

    public static CartLine Normalize(CartLine line) =>
        ToLine(line.Pizza, line.Quantity, line.ExtraIds);

    public static CartSnap ToSnap(CartLine line) =>
        new(line.Pizza.Slug, line.Quantity, line.ExtraIds.ToArray());

    public static IReadOnlyList<CartLine> Hydrate(
        IEnumerable<CartSnap>? snaps,
        IReadOnlyList<PizzaItem> menu)
    {
        if (snaps is null)
        {
            return [];
        }

        var lines = new List<CartLine>();
        foreach (var snap in snaps)
        {
            if (string.IsNullOrWhiteSpace(snap.Slug))
            {
                continue;
            }

            var pizza = menu.FirstOrDefault(p =>
                p.Slug.Equals(snap.Slug, StringComparison.OrdinalIgnoreCase));
            if (pizza is null)
            {
                continue;
            }

            lines.Add(ToLine(pizza, snap.Quantity, snap.Extras));
        }

        return lines;
    }
}
