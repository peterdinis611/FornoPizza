using Forno.Data;
using Forno.Mapping;
using Forno.Models;
using Forno.Validation;
using Microsoft.EntityFrameworkCore;

namespace Forno.Services;

public sealed class MenuService(IDbContextFactory<FornoDbContext> factory) : IMenuService
{
    public async Task<IReadOnlyList<PizzaItem>> AllAsync(CancellationToken cancellation = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellation);
        var rows = await db.Pizzas
            .AsNoTracking()
            .OrderBy(p => p.SortOrder)
            .ToListAsync(cancellation);

        return rows.Select(p => p.ToItem()).ToList();
    }

    public async Task<IReadOnlyList<string>> SlugsAsync(CancellationToken cancellation = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellation);
        return await db.Pizzas
            .AsNoTracking()
            .OrderBy(p => p.SortOrder)
            .Select(p => p.Slug)
            .ToListAsync(cancellation);
    }

    public async Task<IReadOnlyList<PizzaItem>> FeaturedAsync(CancellationToken cancellation = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellation);
        var rows = await db.Pizzas
            .AsNoTracking()
            .Where(p => p.Featured)
            .OrderBy(p => p.SortOrder)
            .ToListAsync(cancellation);

        return rows.Select(p => p.ToItem()).ToList();
    }

    public async Task<PizzaItem?> FindAsync(string slug, CancellationToken cancellation = default)
    {
        slug = InputText.Slug(slug);
        if (slug.Length == 0)
        {
            return null;
        }

        await using var db = await factory.CreateDbContextAsync(cancellation);
        var row = await db.Pizzas
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Slug == slug, cancellation);

        return row?.ToItem();
    }

    public async Task<IReadOnlyList<PizzaItem>> FilterAsync(
        string? query,
        string? tag,
        CancellationToken cancellation = default) =>
        MenuFilter.Apply(await AllAsync(cancellation), query, tag);
}
