using System.Text.RegularExpressions;
using Forno.Domain;

namespace Forno.Validation;

public static class PhoneRules
{
    private static readonly Regex Junk = new(@"[\s\-\.\(\)/]", RegexOptions.Compiled);

    // SK mobil: +421 9xx xxx xxx
    private static readonly Regex Mobile = new(@"^\+4219\d{8}$", RegexOptions.Compiled);

    public static string Normalize(string? value)
    {
        var raw = Junk.Replace(value ?? "", "");
        if (raw.StartsWith("00421", StringComparison.Ordinal))
        {
            raw = "+421" + raw[5..];
        }
        else if (raw.StartsWith("421", StringComparison.Ordinal) && raw.Length == 12)
        {
            raw = "+" + raw;
        }
        else if (raw.StartsWith('0') && raw.Length == 10)
        {
            raw = "+421" + raw[1..];
        }
        else if (raw.Length == 9 && raw.All(char.IsDigit))
        {
            raw = "+421" + raw;
        }

        return raw.Length <= OvenLimits.PhoneMax ? raw : raw[..OvenLimits.PhoneMax];
    }

    public static bool IsValid(string? value)
    {
        var phone = Normalize(value);
        return Mobile.IsMatch(phone);
    }
}
