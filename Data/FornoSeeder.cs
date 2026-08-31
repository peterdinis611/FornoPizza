using Microsoft.EntityFrameworkCore;

namespace Forno.Data;

public static class FornoSeeder
{
    public static async Task SeedAsync(FornoDbContext db, CancellationToken cancellation = default)
    {
        if (await db.Pizzas.AnyAsync(cancellation))
        {
            return;
        }

        var order = 1;
        foreach (var item in MenuSeed.Items)
        {
            db.Pizzas.Add(new Pizza
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
                SortOrder = order++,
            });
        }

        await db.SaveChangesAsync(cancellation);
    }
}
