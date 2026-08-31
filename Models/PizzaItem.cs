namespace Forno.Models;

public sealed record PizzaItem(
    string Slug,
    string Name,
    string Tagline,
    string Description,
    string Ingredients,
    decimal Price,
    string Tone,
    bool Featured = false);
