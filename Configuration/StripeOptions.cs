namespace Forno.Configuration;

public sealed class StripeOptions
{
    public const string Section = "Stripe";

    public string SecretKey { get; set; } = "";
    public string PublishableKey { get; set; } = "";
    public string WebhookSecret { get; set; } = "";

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(SecretKey);
}
