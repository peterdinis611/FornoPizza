using System.Net.Mail;
using Forno.Domain;

namespace Forno.Validation;

public static class EmailRules
{
    public static bool IsValid(string? value)
    {
        var email = InputText.Email(value);
        if (email.Length is < 6 or > OvenLimits.EmailMax)
        {
            return false;
        }

        return MailAddress.TryCreate(email, out var parsed)
            && parsed.Address.Equals(email, StringComparison.OrdinalIgnoreCase);
    }
}
