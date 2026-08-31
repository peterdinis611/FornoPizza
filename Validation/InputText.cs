using System.Globalization;
using Forno.Domain;

namespace Forno.Validation;

public static class InputText
{
    public static string Collapse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "";
        }

        var parts = value
            .Trim()
            .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        return string.Join(' ', parts);
    }

    public static string Clamp(string value, int max) =>
        value.Length <= max ? value : value[..max];

    public static string Name(string? value) =>
        Clamp(Collapse(value), OvenLimits.NameMax);

    public static string Address(string? value) =>
        Clamp(Collapse(value), OvenLimits.AddressMax);

    public static string Note(string? value)
    {
        var text = (value ?? "").Trim();
        return Clamp(text, OvenLimits.NoteMax);
    }

    public static string Email(string? value) =>
        Clamp((value ?? "").Trim().ToLowerInvariant(), OvenLimits.EmailMax);

    public static string Slug(string? value) =>
        Clamp((value ?? "").Trim().ToLowerInvariant(), OvenLimits.SlugMax);

    public static string Query(string? value) =>
        Clamp((value ?? "").Trim(), OvenLimits.QueryMax);

    public static bool HasLetter(string value) =>
        value.Any(ch => char.GetUnicodeCategory(ch) is UnicodeCategory.LowercaseLetter
            or UnicodeCategory.UppercaseLetter
            or UnicodeCategory.TitlecaseLetter);
}
