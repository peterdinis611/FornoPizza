using Forno.Models;
using Forno.Validation;

namespace Forno.Mapping;

public static class MenuFilter
{
    public static IReadOnlyList<PizzaItem> Apply(IReadOnlyList<PizzaItem> items, string? query, string? tag)
    {
        IEnumerable<PizzaItem> result = items;
        query = InputText.Query(query);
        tag = InputText.Collapse(tag);

        if (!string.IsNullOrWhiteSpace(tag))
        {
            result = result.Where(p => p.Tags.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)));
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            result = result.Where(p =>
                p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                p.Tagline.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                p.Ingredients.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        return result.ToList();
    }
}
