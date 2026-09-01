using System.Text.Json;
using Forno.Models;

namespace Forno.Mapping;

public static class MenuSearchIndex
{
    public static string ToJson(IEnumerable<PizzaItem> items) =>
        JsonSerializer.Serialize(items.Select(row => new
        {
            row.Slug,
            row.Name,
            row.Tagline,
            row.Description,
            row.Ingredients,
            row.Tags,
            row.Tone,
            Price = row.Price.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)
        }));
}
