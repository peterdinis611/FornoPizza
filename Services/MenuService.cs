using Forno.Data;
using Forno.Models;
using Microsoft.EntityFrameworkCore;

namespace Forno.Services;

public sealed class MenuService(IDbContextFactory<FornoDbContext> factory)
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
        await using var db = await factory.CreateDbContextAsync(cancellation);
        var row = await db.Pizzas
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Slug == slug, cancellation);

        return row?.ToItem();
    }

    public async Task<IReadOnlyList<PizzaItem>> FilterAsync(string? query, string? tag, CancellationToken cancellation = default)
    {
        var items = await AllAsync(cancellation);

        if (!string.IsNullOrWhiteSpace(tag))
        {
            var needle = tag.Trim();
            items = items.Where(p => p.Tags.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Any(t => t.Equals(needle, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            var q = query.Trim();
            items = items.Where(p =>
                p.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                p.Tagline.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                p.Ingredients.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(q, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return items;
    }
}
