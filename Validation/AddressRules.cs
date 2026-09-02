using Forno.Domain;

namespace Forno.Validation;

public static class AddressRules
{
    public static string Normalize(string? value) =>
        InputText.Address(value);

    public static bool IsValid(string? value)
    {
        var address = Normalize(value);
        if (address.Length < OvenLimits.AddressMin || address.Length > OvenLimits.AddressMax)
        {
            return false;
        }

        if (!InputText.HasLetter(address))
        {
            return false;
        }

        // Doručenie potrebuje číslo domu / bytu.
        if (!address.Any(char.IsDigit))
        {
            return false;
        }

        var words = address.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return words.Length >= 2;
    }
}
