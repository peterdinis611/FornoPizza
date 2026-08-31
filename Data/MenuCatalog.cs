using Forno.Models;

namespace Forno.Data;

public static class MenuCatalog
{
    public static IReadOnlyList<PizzaItem> All { get; } =
    [
        new(
            "margherita",
            "Margherita",
            "Kráľovná peci",
            "Cesto 48 hodín, San Marzano, fior di latte, bazalka z dvora. Nič navyše.",
            "San Marzano · fior di latte · bazalka · olivový olej",
            9.90m,
            "tomato",
            Featured: true),
        new(
            "marinara",
            "Marinara",
            "Bez syra, s dymom",
            "Najstarší list z Neapola. Cesnak, oregano, olej — a teplo, ktoré ostane v kôre.",
            "San Marzano · cesnak · oregano · extra virgin",
            8.50m,
            "olive"),
        new(
            "diavola",
            "Diavola",
            "Pikantná, nie agresívna",
            "Calabrijská saláma, med z úľa za mestom, chili olej. Pec ju spečie do lesku.",
            "fior di latte · salame piccante · chili olej · med",
            12.90m,
            "char",
            Featured: true),
        new(
            "quattro-formaggi",
            "Quattro formaggi",
            "Štyri syry, jedna kôra",
            "Mozzarella, gorgonzola, pecorino, ricotta. Krémová, ale stále o ceste.",
            "fior di latte · gorgonzola · pecorino · ricotta",
            13.50m,
            "cream"),
        new(
            "funghi",
            "Prosciutto e funghi",
            "Les a údené",
            "Šunka, šampióny, petržlen. Klasika, ktorú pec nesmie prepiecť.",
            "fior di latte · prosciutto cotto · šampióny · petržlen",
            12.40m,
            "olive"),
        new(
            "capricciosa",
            "Capricciosa",
            "Všetko, čo pec unesie",
            "Šunka, artičoky, olivy, šampióny. Rozmar, nie chaos.",
            "fior di latte · šunka · artičoky · olivy · šampióny",
            13.20m,
            "tomato",
            Featured: true),
    ];

    public static PizzaItem? Find(string slug) =>
        All.FirstOrDefault(p => p.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));

    public static IEnumerable<PizzaItem> Featured => All.Where(p => p.Featured);
}
