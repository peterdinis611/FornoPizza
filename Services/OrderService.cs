using Forno.Data;
using Forno.Models;
using Microsoft.EntityFrameworkCore;

namespace Forno.Services;

public sealed class OrderService(IDbContextFactory<FornoDbContext> factory)
{
    public async Task<OvenOrder> PlaceAsync(
        string name,
        string phone,
        string address,
        string note,
        IReadOnlyList<CartLine> lines,
        CancellationToken cancellation = default)
    {
        if (lines.Count == 0)
        {
            throw new InvalidOperationException("Košík je prázdny.");
        }

        await using var db = await factory.CreateDbContextAsync(cancellation);

        var slugs = lines.Select(l => l.Pizza.Slug).Distinct().ToList();
        var pizzas = await db.Pizzas
            .Where(p => slugs.Contains(p.Slug))
            .ToDictionaryAsync(p => p.Slug, cancellation);

        var order = new OvenOrder
        {
            CreatedAt = DateTimeOffset.UtcNow,
            Name = name.Trim(),
            Phone = phone.Trim(),
            Address = address.Trim(),
            Note = note.Trim(),
            Status = "prijata",
        };

        foreach (var line in lines)
        {
            if (!pizzas.TryGetValue(line.Pizza.Slug, out var pizza))
            {
                continue;
            }

            var qty = Math.Clamp(line.Quantity, 1, 12);
            order.Lines.Add(new OrderLine
            {
                PizzaId = pizza.Id,
                PizzaSlug = pizza.Slug,
                PizzaName = pizza.Name,
                UnitPrice = pizza.Price,
                Quantity = qty,
            });
        }

        if (order.Lines.Count == 0)
        {
            throw new InvalidOperationException("V peci už tieto listy nie sú.");
        }

        order.Total = order.Lines.Sum(l => l.UnitPrice * l.Quantity);
        db.Orders.Add(order);
        await db.SaveChangesAsync(cancellation);
        return order;
    }
}
