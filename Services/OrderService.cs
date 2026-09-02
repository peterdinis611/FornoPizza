using Forno.Contracts;
using Forno.Data;
using Forno.Domain;
using Forno.Interfaces;
using Forno.Mapping;
using Forno.Validation;
using Microsoft.EntityFrameworkCore;

namespace Forno.Services;

public sealed class OrderService(
    IDbContextFactory<FornoDbContext> factory,
    IPaymentService payment,
    IOrderBus bus) : IOrderService
{
    public async Task<Result<OrderReceipt>> PlaceAsync(
        PlaceOrderRequest request,
        CancellationToken cancellation = default)
    {
        var errors = CheckoutValidator.Validate(request);
        if (errors.Count > 0)
        {
            return Result<OrderReceipt>.Fail(errors);
        }

        await using var db = await factory.CreateDbContextAsync(cancellation);

        var slugs = request.Lines
            .Select(line => InputText.Slug(line.Pizza.Slug))
            .Where(slug => slug.Length > 0)
            .Distinct()
            .ToList();

        var pizzas = await db.Pizzas
            .Where(p => slugs.Contains(p.Slug))
            .ToDictionaryAsync(p => p.Slug, cancellation);

        var lines = new List<OrderLine>();
        foreach (var line in request.Lines)
        {
            var slug = InputText.Slug(line.Pizza.Slug);
            if (!pizzas.TryGetValue(slug, out var pizza))
            {
                return Result<OrderReceipt>.Fail(
                    "cart",
                    $"List {line.Pizza.Name} v peci už nie je.");
            }

            lines.Add(OrderMapper.ToLine(line, pizza));
        }

        if (lines.Count == 0)
        {
            return Result<OrderReceipt>.Fail("cart", "Košík je prázdny.");
        }

        var status = payment.IsEnabled ? OrderStatus.PendingPayment : OrderStatus.Accepted;
        var order = OrderMapper.ToOrder(request, lines, status);
        db.Orders.Add(order);

        try
        {
            await db.SaveChangesAsync(cancellation);
        }
        catch (DbUpdateException)
        {
            return Result<OrderReceipt>.Fail("order", "Lístok sa nepodarilo zapísať. Skúste znova.");
        }

        if (order.Status == OrderStatus.Accepted)
        {
            await bus.PublishKitchenTicketAsync(order, OrderEvents.Placed, cancellation);
        }

        return Result<OrderReceipt>.Ok(OrderMapper.ToReceipt(order));
    }
}
