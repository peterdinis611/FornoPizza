namespace Forno.Models;

public sealed record OvenExtra(
    string Id,
    string Name,
    string Note,
    decimal Price,
    string Swatch,
    string Allergen = "");

public static class OvenExtras
{
    public const int MaxOnLeaf = 6;

    public static IReadOnlyList<OvenExtra> All { get; } =
    [
        new("kukurica", "Kukurica", "Sladké zrná. Pec ich len zahreje.", 1.00m, "#e8c04a"),
        new("syr", "Fior di latte", "Ešte jedna vrstva. Kôra ostane suchá.", 1.40m, "#fff6de", "mlieko"),
        new("olivy", "Olivy", "Čierne, celé. Soľ ide z nich.", 0.90m, "#1c1612"),
        new("chilli", "Chilli", "Olej až na konci. Lesk, nie trest.", 0.70m, "#c23018"),
        new("sunka", "Šunka", "Ružová ostane ružová.", 1.50m, "#d8a090", "bravčové"),
        new("sampiony", "Šampióny", "Suché, tenké. Voda kôru utopí.", 1.10m, "#c4b48a"),
        new("cesnak", "Cesnak", "Tenké plátky. Pec ho opečie.", 0.60m, "#efe6c8"),
        new("bazalka", "Bazalka", "Až po ohni. Inak zhorí.", 0.50m, "#3a4a2c"),
        new("cibula", "Cibuľa", "Sladká, tenká. Nesmie byť surová.", 0.80m, "#dcc4d0"),
        new("gorgonzola", "Gorgonzola", "Pruhy, nie lyžica.", 1.60m, "#9bb8c4", "mlieko"),
        new("articoky", "Artičoky", "Odkvapkané. Šťava zhasí kôru.", 1.30m, "#c4b48a"),
        new("med", "Med", "Nitka na kôru. Dymová sladkosť.", 0.80m, "#e8b86a")
    ];

    public static OvenExtra? Find(string id) =>
        All.FirstOrDefault(x => x.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

    public static IReadOnlyList<string> Normalize(IEnumerable<string>? ids) =>
        (ids ?? [])
            .Where(id => Find(id) is not null)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(MaxOnLeaf)
            .OrderBy(id => id, StringComparer.OrdinalIgnoreCase)
            .ToList();

    public static decimal Sum(IEnumerable<string> ids) =>
        ids.Sum(id => Find(id)?.Price ?? 0);

    public static string Label(IEnumerable<string> ids) =>
        string.Join(" · ", ids.Select(id => Find(id)?.Name).OfType<string>());

    public static string Allergens(IEnumerable<string> ids) =>
        string.Join(" · ", ids.Select(id => Find(id)?.Allergen).Where(a => !string.IsNullOrWhiteSpace(a)).Distinct());

    public static bool Same(IEnumerable<string> left, IEnumerable<string> right) =>
        Normalize(left).SequenceEqual(Normalize(right), StringComparer.OrdinalIgnoreCase);
}
