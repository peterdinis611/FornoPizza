using System.Text.RegularExpressions;
using Forno.Domain;

namespace Forno.Validation;

public static class NameRules
{
    private static readonly Regex Junk = new(@"[^\p{L}\s\-'.]", RegexOptions.Compiled);

    public static string Normalize(string? value) =>
        InputText.Name(value);

    public static bool IsValid(string? value)
    {
        var name = Normalize(value);
        if (name.Length < OvenLimits.NameMin || name.Length > OvenLimits.NameMax)
        {
            return false;
        }

        if (!InputText.HasLetter(name))
        {
            return false;
        }

        if (name.Any(char.IsDigit))
        {
            return false;
        }

        if (Junk.IsMatch(name))
        {
            return false;
        }

        var letters = name.Count(ch => char.IsLetter(ch));
        return letters >= OvenLimits.NameMin;
    }
}
