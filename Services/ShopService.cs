using Forno.Contracts;
using Forno.Data;
using Forno.Domain;
using Forno.Mapping;
using Forno.Models;
using Forno.Validation;
using Microsoft.EntityFrameworkCore;

namespace Forno.Services;

public sealed class ShopService(IDbContextFactory<FornoDbContext> factory) : IShopService
{
    public async Task<PizzaItem?> DaySpecialAsync(CancellationToken cancellation = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellation);
        var slug = await db.Settings
            .AsNoTracking()
            .Where(setting => setting.Key == ShopKeys.DaySpecial)
            .Select(setting => setting.Value)
            .FirstOrDefaultAsync(cancellation);

        slug = InputText.Slug(slug);
        if (slug.Length == 0)
        {
            return null;
        }

        var row = await db.Pizzas
            .AsNoTracking()
            .FirstOrDefaultAsync(pizza => pizza.Slug == slug, cancellation);

        return row?.ToItem();
    }

    public async Task<Result<string>> SetDaySpecialAsync(string slug, CancellationToken cancellation = default)
    {
        slug = InputText.Slug(slug);
        if (slug.Length == 0)
        {
            return Result<string>.Fail("slug", "Vyberte list z menu.");
        }

        await using var db = await factory.CreateDbContextAsync(cancellation);
        if (!await db.Pizzas.AnyAsync(pizza => pizza.Slug == slug, cancellation))
        {
            return Result<string>.Fail("slug", "Tento list v peci nie je.");
        }

        var setting = await db.Settings
            .FirstOrDefaultAsync(row => row.Key == ShopKeys.DaySpecial, cancellation);

        if (setting is null)
        {
            db.Settings.Add(new OvenSetting
            {
                Key = ShopKeys.DaySpecial,
                Value = slug
            });
        }
        else
        {
            setting.Value = slug;
        }

        await db.SaveChangesAsync(cancellation);
        return Result<string>.Ok(slug);
    }
}
