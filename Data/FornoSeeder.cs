using Forno.Mapping;
using Forno.Domain;
using Microsoft.EntityFrameworkCore;

namespace Forno.Data;

public static class FornoSeeder
{
    public static async Task SeedAsync(FornoDbContext db, CancellationToken cancellation = default)
    {
        if (!await db.Pizzas.AnyAsync(cancellation))
        {
            var order = 1;
            foreach (var item in MenuSeed.Items)
            {
                db.Pizzas.Add(PizzaMapper.ToEntity(item, order++));
            }

            await db.SaveChangesAsync(cancellation);
        }

        if (!await db.Settings.AnyAsync(setting => setting.Key == ShopKeys.DaySpecial, cancellation))
        {
            db.Settings.Add(new OvenSetting
            {
                Key = ShopKeys.DaySpecial,
                Value = "margherita"
            });

            await db.SaveChangesAsync(cancellation);
        }
    }
}
