using System.Net.Mail;
using System.Text.RegularExpressions;
using Forno.Domain;

namespace Forno.Validation;

public static class EmailRules
{
    private static readonly Regex Shape = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static string Normalize(string? value) =>
        InputText.Email(value);

    public static bool IsValid(string? value)
    {
        var email = Normalize(value);
        if (email.Length is < 6 or > OvenLimits.EmailMax)
        {
            return false;
        }

        if (email.Contains(' ') || email.Count(ch => ch == '@') != 1)
        {
            return false;
        }

        if (!Shape.IsMatch(email))
        {
            return false;
        }

        if (!MailAddress.TryCreate(email, out var parsed))
        {
            return false;
        }

        return parsed.Address.Equals(email, StringComparison.OrdinalIgnoreCase)
            && parsed.Host.Contains('.')
            && !parsed.Host.StartsWith('.')
            && !parsed.Host.EndsWith('.');
    }
}
