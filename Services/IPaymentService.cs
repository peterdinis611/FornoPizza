using Forno.Contracts;

namespace Forno.Services;

public interface IPaymentService
{
    bool IsEnabled { get; }

    string? PublishableKey { get; }

    Task<Result<string>> CreateCheckoutSessionAsync(int orderId, string baseUrl, CancellationToken cancellation = default);

    Task<Result<OrderReceipt>> ConfirmSessionAsync(string sessionId, CancellationToken cancellation = default);

    Task HandleWebhookAsync(string json, string signature, CancellationToken cancellation = default);
}
