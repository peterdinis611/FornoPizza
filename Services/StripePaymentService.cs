using Forno.Configuration;
using Forno.Contracts;
using Forno.Data;
using Forno.Domain;
using Forno.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Forno.Services;

public sealed class StripePaymentService(
    IOptions<StripeOptions> options,
    IDbContextFactory<FornoDbContext> factory,
    ILogger<StripePaymentService> logger) : IPaymentService
{
    private readonly StripeOptions _options = options.Value;

    public bool IsEnabled => _options.IsConfigured;

    public string? PublishableKey =>
        string.IsNullOrWhiteSpace(_options.PublishableKey) ? null : _options.PublishableKey.Trim();

    public async Task<Result<string>> CreateCheckoutSessionAsync(
        int orderId,
        string baseUrl,
        CancellationToken cancellation = default)
    {
        if (!IsEnabled)
        {
            return Result<string>.Fail("payment", "Platba kartou nie je nakonfigurovaná.");
        }

        await using var db = await factory.CreateDbContextAsync(cancellation);
        var order = await db.Orders
            .Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellation);

        if (order is null)
        {
            return Result<string>.Fail("order", "Objednávka sa nenašla.");
        }

        if (order.Status == OrderStatus.Paid)
        {
            return Result<string>.Fail("order", "Objednávka je už zaplatená.");
        }

        StripeConfiguration.ApiKey = _options.SecretKey.Trim();
        var root = baseUrl.TrimEnd('/');

        var lineItems = order.Lines.Select(line => new SessionLineItemOptions
        {
            Quantity = line.Quantity,
            PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = "eur",
                UnitAmount = ToCents(line.UnitPrice),
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = LineName(line)
                }
            }
        }).ToList();

        var sessionService = new SessionService();
        var session = await sessionService.CreateAsync(new SessionCreateOptions
        {
            Mode = "payment",
            LineItems = lineItems,
            SuccessUrl = $"{root}/checkout/success?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{root}/checkout?cancelled=1",
            Metadata = new Dictionary<string, string>
            {
                ["order_id"] = order.Id.ToString()
            },
            PaymentIntentData = new SessionPaymentIntentDataOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    ["order_id"] = order.Id.ToString()
                }
            }
        }, cancellationToken: cancellation);

        if (string.IsNullOrWhiteSpace(session.Url))
        {
            return Result<string>.Fail("payment", "Stripe nevrátil platobnú stránku.");
        }

        order.StripeSessionId = session.Id;
        order.Status = OrderStatus.PendingPayment;
        await db.SaveChangesAsync(cancellation);

        return Result<string>.Ok(session.Url);
    }

    public async Task<Result<OrderReceipt>> ConfirmSessionAsync(
        string sessionId,
        CancellationToken cancellation = default)
    {
        if (!IsEnabled)
        {
            return Result<OrderReceipt>.Fail("payment", "Platba kartou nie je nakonfigurovaná.");
        }

        sessionId = (sessionId ?? "").Trim();
        if (sessionId.Length == 0)
        {
            return Result<OrderReceipt>.Fail("session", "Chýba platobná relácia.");
        }

        StripeConfiguration.ApiKey = _options.SecretKey.Trim();
        var session = await new SessionService().GetAsync(sessionId, cancellationToken: cancellation);
        if (!string.Equals(session.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase))
        {
            return Result<OrderReceipt>.Fail("payment", "Platba ešte neprebehla.");
        }

        var orderId = ReadOrderId(session.Metadata);
        if (orderId is null)
        {
            return Result<OrderReceipt>.Fail("order", "Objednávku sa nepodarilo priradiť.");
        }

        return await MarkPaidAsync(orderId.Value, session.Id, cancellation);
    }

    public async Task HandleWebhookAsync(
        string json,
        string signature,
        CancellationToken cancellation = default)
    {
        if (!IsEnabled || string.IsNullOrWhiteSpace(_options.WebhookSecret))
        {
            return;
        }

        StripeConfiguration.ApiKey = _options.SecretKey.Trim();
        var stripeEvent = EventUtility.ConstructEvent(
            json,
            signature,
            _options.WebhookSecret.Trim());

        if (stripeEvent.Type != Events.CheckoutSessionCompleted)
        {
            return;
        }

        if (stripeEvent.Data.Object is not Session session)
        {
            return;
        }

        if (!string.Equals(session.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var orderId = ReadOrderId(session.Metadata);
        if (orderId is null)
        {
            logger.LogWarning("Stripe webhook bez order_id pre reláciu {SessionId}", session.Id);
            return;
        }

        await MarkPaidAsync(orderId.Value, session.Id, cancellation);
    }

    private async Task<Result<OrderReceipt>> MarkPaidAsync(
        int orderId,
        string sessionId,
        CancellationToken cancellation)
    {
        await using var db = await factory.CreateDbContextAsync(cancellation);
        var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == orderId, cancellation);
        if (order is null)
        {
            return Result<OrderReceipt>.Fail("order", "Objednávka sa nenašla.");
        }

        if (order.Status == OrderStatus.Paid)
        {
            return Result<OrderReceipt>.Ok(OrderMapper.ToReceipt(order));
        }

        order.Status = OrderStatus.Paid;
        order.StripeSessionId = sessionId;
        await db.SaveChangesAsync(cancellation);

        return Result<OrderReceipt>.Ok(OrderMapper.ToReceipt(order));
    }

    private static int? ReadOrderId(IReadOnlyDictionary<string, string>? metadata)
    {
        if (metadata is null || !metadata.TryGetValue("order_id", out var raw))
        {
            return null;
        }

        return int.TryParse(raw, out var id) ? id : null;
    }

    private static string LineName(OrderLine line) =>
        string.IsNullOrWhiteSpace(line.Extras)
            ? line.PizzaName
            : $"{line.PizzaName} · {line.Extras}";

    private static long ToCents(decimal amount) =>
        (long)Math.Round(amount * 100m, MidpointRounding.AwayFromZero);
}
