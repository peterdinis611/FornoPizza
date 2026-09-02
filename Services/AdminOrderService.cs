using Forno.Contracts;
using Forno.Data;
using Forno.Domain;
using Forno.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Forno.Services;

public sealed class AdminOrderService(IDbContextFactory<FornoDbContext> factory) : IAdminOrderService
{
    public async Task<AdminStats> StatsAsync(CancellationToken cancellation = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellation);
        var start = StartOfLocalDayUtc();

        var today = await db.Orders
            .AsNoTracking()
            .Where(o => o.CreatedAt >= start && o.Status != OrderStatus.Cancelled)
            .ToListAsync(cancellation);

        var kitchen = await db.Orders
            .AsNoTracking()
            .CountAsync(o =>
                o.Status == OrderStatus.Paid
                || o.Status == OrderStatus.Accepted
                || o.Status == OrderStatus.Baking
                || o.Status == OrderStatus.Ready, cancellation);

        var pending = await db.Orders
            .AsNoTracking()
            .CountAsync(o => o.Status == OrderStatus.PendingPayment, cancellation);

        var all = await db.Orders.AsNoTracking().CountAsync(cancellation);

        var revenue = today
            .Where(o => o.Status is OrderStatus.Paid
                or OrderStatus.Accepted
                or OrderStatus.Baking
                or OrderStatus.Ready
                or OrderStatus.Done)
            .Sum(o => o.Total);

        return new AdminStats(today.Count, revenue, kitchen, pending, all);
    }

    public async Task<AdminOrderPage> ListAsync(
        string? status = null,
        int take = 80,
        CancellationToken cancellation = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellation);
        take = Math.Clamp(take, 1, 200);

        var query = db.Orders
            .AsNoTracking()
            .Include(o => o.Lines)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status)
            && OrderStatus.All.Contains(status, StringComparer.OrdinalIgnoreCase))
        {
            query = query.Where(o => o.Status == status);
        }

        var rows = await query
            .OrderByDescending(o => o.CreatedAt)
            .Take(take)
            .ToListAsync(cancellation);

        return new AdminOrderPage(rows.Select(Map).ToList(), await StatsAsync(cancellation));
    }

    public async Task<AdminOrder?> GetAsync(int id, CancellationToken cancellation = default)
    {
        await using var db = await factory.CreateDbContextAsync(cancellation);
        var order = await db.Orders
            .AsNoTracking()
            .Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.Id == id, cancellation);

        return order is null ? null : Map(order);
    }

    public async Task<Result> SetStatusAsync(
        int id,
        string status,
        CancellationToken cancellation = default)
    {
        status = (status ?? "").Trim().ToLowerInvariant();
        if (!OrderStatus.All.Contains(status, StringComparer.OrdinalIgnoreCase))
        {
            return Result.Fail("status", "Neznámy stav objednávky.");
        }

        await using var db = await factory.CreateDbContextAsync(cancellation);
        var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == id, cancellation);
        if (order is null)
        {
            return Result.Fail("order", "Objednávka sa nenašla.");
        }

        if (!OrderStatus.CanTransition(order.Status, status))
        {
            return Result.Fail(
                "status",
                $"Zo stavu „{OrderStatus.Label(order.Status)}“ nejde na „{OrderStatus.Label(status)}“.");
        }

        order.Status = status;

        try
        {
            await db.SaveChangesAsync(cancellation);
        }
        catch (DbUpdateException)
        {
            return Result.Fail("order", "Stav sa nepodarilo uložiť.");
        }

        return Result.Ok();
    }

    private static AdminOrder Map(OvenOrder order) =>
        new(
            order.Id,
            order.CreatedAt,
            order.Name,
            order.Email,
            order.Phone,
            order.Address,
            order.Note,
            order.Fulfillment,
            order.Status,
            order.Total,
            order.StripeSessionId,
            order.Lines
                .OrderBy(l => l.Id)
                .Select(l => new AdminOrderLine(
                    l.PizzaSlug,
                    l.PizzaName,
                    l.Quantity,
                    l.UnitPrice,
                    l.Extras,
                    l.LineTotal))
                .ToList());

    private static DateTimeOffset StartOfLocalDayUtc()
    {
        var local = TimeZoneInfo.Local;
        var nowLocal = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, local);
        var startLocal = new DateTimeOffset(nowLocal.Date, nowLocal.Offset);
        return startLocal.ToUniversalTime();
    }
}
