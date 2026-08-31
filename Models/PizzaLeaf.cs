namespace Forno.Models;

public sealed record LeafLayer(string Name, string Note);

public sealed record PizzaLeaf(
    string Number,
    string Quote,
    string BakeNote,
    string PairsWith,
    string Allergens,
    IReadOnlyList<LeafLayer> Layers,
    string Weight = "450 g",
    string Flour = "tipo 00",
    string Wood = "buk",
    string Diameter = "Ø 32 cm",
    string Heat = "800°",
    string Time = "90 s",
    string Dough = "48 h");

public static class PizzaLeaves
{
    public static PizzaLeaf For(string slug) => slug switch
    {
        "margherita" => new(
            "01",
            "Nič navyše. Pec má rada ticho.",
            "San Marzano ide do pece riedky. Bazalka až vonku, kým kôra ešte praská.",
            "List z ruky. Alebo červená, ktorá neprekričí paradajku.",
            "Pšenica · mlieko (fior di latte)",
            [
                new("San Marzano", "Riedka, sladká, bez cukru. Pec ju spečie do lesku."),
                new("Fior di latte", "Mäkký, nie gumový. Len zaleskne."),
                new("Bazalka", "Až po ohni. Inak zhorí na prach."),
                new("Olivový olej", "Nitka, nie kaluž. Kôra ostane suchá.")
            ],
            Weight: "380 g"),
        "marinara" => new(
            "02",
            "Najstarší list. Syra sa nepýta.",
            "Bez syra, s cesnakom. Pec ju drží kratšie, aby olej zostal lesklý.",
            "Olivy na stole, nič smotanové. Táto kôra chce soľ a ticho.",
            "Pšenica · bez syra",
            [
                new("San Marzano", "Hustejšia ako na Margherite. Cesnak ju nesmie prekričať."),
                new("Cesnak", "Tenké plátky. Pec ho opečie, nespáli."),
                new("Oregano", "Suché, štipka. Mokré by zhasilo kôru."),
                new("Extra virgin", "Až vonku. V peci by horkol.")
            ],
            Weight: "360 g"),
        "diavola" => new(
            "03",
            "Pálivé má byť lesk, nie trest.",
            "Chili olej až na konci. Med sa v 800° zmení na lesk, nie na sladkosť.",
            "Horká voda, alebo pivo, ktoré znesie dym. Nie mlieko.",
            "Pšenica · mlieko · bravčové (saláma)",
            [
                new("Fior di latte", "Menej ako na klasike. Saláma potrebuje miesto."),
                new("Salame piccante", "Calábria. Pec ju spečie do oleja, nie do uhlia."),
                new("Chili olej", "Až po ohni. V peci by zhorkol."),
                new("Med", "Nitka na kôru. Sladkosť ostane dymová.")
            ],
            Weight: "430 g"),
        "quattro-formaggi" => new(
            "04",
            "Štyri syry, jedna kôra. Pec nesmie prepiecť.",
            "Štyri syry, ale pec nesmie prepiecť. Gorgonzola ostane pruh, nie kaluž.",
            "Hruška, alebo suché biele. Krém chce partnera, nie ďalší syr.",
            "Pšenica · mlieko (štyri syry)",
            [
                new("Fior di latte", "Základ. Drží ostatné, aby nestiekli."),
                new("Gorgonzola", "Pruhy, nie lyžica. Pec ju len zahreje."),
                new("Pecorino", "Soľ a ostrosť. Trochu stačí."),
                new("Ricotta", "Bodky. V peci ostane mäkká.")
            ],
            Weight: "440 g"),
        "funghi" => new(
            "05",
            "Les a údené. Pec ich nesmie prepiecť.",
            "Šampióny idú suché. Pec ich opečie, šunka ostane ružová, nie tvrdá.",
            "Lesný čaj, alebo červené, ktoré neprekričí údené.",
            "Pšenica · mlieko · bravčové (šunka)",
            [
                new("Fior di latte", "Tenká vrstva. Huby potrebujú suchú kôru."),
                new("Prosciutto cotto", "Po peci ružová. Prepiecť ju nesmie."),
                new("Šampióny", "Suché, tenké. Voda z nich kôru utopí."),
                new("Petržlen", "Až vonku. Zelené ostane zelené.")
            ],
            Weight: "420 g"),
        "capricciosa" => new(
            "06",
            "Všetko, čo pec unesie — ale po jednom.",
            "Všetko, čo pec unesie — ale po jednom. Oliva a artičok nesmú utopiť kôru.",
            "Horký čaj z citróna. Alebo niečo suché, kým ešte horí stôl.",
            "Pšenica · mlieko · bravčové (šunka)",
            [
                new("Fior di latte", "Menej ako na Margherite. List je plný."),
                new("Šunka", "Ružová ostane ružová. Pec ju len zahreje."),
                new("Artičoky", "Odkvapkané. Šťava by zhasila kôru."),
                new("Olivy", "Čierne, celé. Soľ ide z nich, nie z cesta."),
                new("Šampióny", "Suché, tenké. Medzi šunkou, nie na nej.")
            ],
            Weight: "450 g"),
        _ => new(
            "—",
            "Buk, 800°, deväťdesiat sekúnd. Potom už len kôra.",
            "Buk, 800°, deväťdesiat sekúnd. Potom už len kôra.",
            "To, čo máte na stole. Pec nežiada obrad.",
            "Pšenica · v poznámke k objednávke napíšte viac",
            [])
    };
}
