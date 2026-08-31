using Forno.Data;
using Forno.Models;

namespace Forno.Mapping;

public static class PizzaMapper
{
    public static PizzaItem ToItem(this Pizza pizza) =>
        new(
            pizza.Slug,
            pizza.Name,
            pizza.Tagline,
            pizza.Description,
            pizza.Ingredients,
            pizza.Price,
            pizza.Tone,
            pizza.Featured,
            pizza.Tags);

    public static Pizza ToEntity(PizzaItem item, int sortOrder) =>
        new()
        {
            Slug = item.Slug,
            Name = item.Name,
            Tagline = item.Tagline,
            Description = item.Description,
            Ingredients = item.Ingredients,
            Price = item.Price,
            Tone = item.Tone,
            Featured = item.Featured,
            Tags = item.Tags,
            SortOrder = sortOrder
        };
}
