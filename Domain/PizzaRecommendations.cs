namespace Forno.Domain;

public static class PizzaRecommendations
{
    public static IReadOnlyList<string> For(string slug) =>
        (slug ?? "").Trim().ToLowerInvariant() switch
        {
            "margherita" => ["bazalka", "syr"],
            "marinara" => ["cesnak", "chilli"],
            "diavola" => ["chilli", "med"],
            "quattro-formaggi" => ["gorgonzola", "med"],
            "funghi" => ["sampiony", "syr"],
            "capricciosa" => ["olivy", "syr"],
            _ => []
        };
}
