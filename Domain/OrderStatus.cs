namespace Forno.Domain;

public static class OrderStatus
{
    public const string PendingPayment = "caka_platbu";
    public const string Paid = "zaplatena";
    public const string Accepted = "prijata";
    public const string Baking = "peci";
    public const string Ready = "hotova";
    public const string Done = "vydana";
    public const string Cancelled = "zrusena";

    public static readonly IReadOnlyList<string> All =
    [
        PendingPayment,
        Paid,
        Accepted,
        Baking,
        Ready,
        Done,
        Cancelled
    ];

    public static string Label(string? status) => status switch
    {
        PendingPayment => "Čaká platbu",
        Paid => "Zaplatená",
        Accepted => "Prijatá",
        Baking => "V peci",
        Ready => "Hotová",
        Done => "Vydaná",
        Cancelled => "Zrušená",
        _ => status ?? "—"
    };

    public static bool IsKitchenActive(string? status) =>
        status is Paid or Accepted or Baking or Ready;

    public static bool CanTransition(string from, string to)
    {
        if (string.Equals(from, to, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (to == Cancelled)
        {
            return from is not Done and not Cancelled;
        }

        return (from, to) switch
        {
            (PendingPayment, Paid) => true,
            (PendingPayment, Accepted) => true,
            (Paid, Accepted) => true,
            (Paid, Baking) => true,
            (Accepted, Baking) => true,
            (Baking, Ready) => true,
            (Ready, Done) => true,
            _ => false
        };
    }

    public static IReadOnlyList<string> Next(string status) => status switch
    {
        PendingPayment => [Paid, Accepted, Cancelled],
        Paid => [Accepted, Baking, Cancelled],
        Accepted => [Baking, Cancelled],
        Baking => [Ready, Cancelled],
        Ready => [Done, Cancelled],
        Done => [],
        Cancelled => [],
        _ => [Accepted, Baking, Ready, Done, Cancelled]
    };
}
