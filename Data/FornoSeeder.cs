using Forno.Mapping;
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
            db.Pizzas.Add(PizzaMapper.ToEntity(item, order++));
        }

        await db.SaveChangesAsync(cancellation);
    }
}
