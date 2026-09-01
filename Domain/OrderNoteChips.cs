namespace Forno.Domain;

public static class OrderNoteChips
{
    public static IReadOnlyList<string> All { get; } =
    [
        "Bez lepku",
        "Bez syra",
        "Bez mäsa",
        "Bez cibule",
        "Bez cesnaku",
        "Extra horúca",
        "Zvonček nefunguje"
    ];

    public static string Merge(IEnumerable<string> chips, string? extra)
    {
        var parts = chips
            .Where(chip => !string.IsNullOrWhiteSpace(chip))
            .Select(chip => chip.Trim())
            .ToList();

        var note = (extra ?? "").Trim();
        if (note.Length > 0)
        {
            parts.Add(note);
        }

        return string.Join(" · ", parts);
    }
}
