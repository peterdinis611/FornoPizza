namespace Forno.Data;

public sealed class Pizza
{
    public int Id { get; set; }
    public string Slug { get; set; } = "";
    public string Name { get; set; } = "";
    public string Tagline { get; set; } = "";
    public string Description { get; set; } = "";
    public string Ingredients { get; set; } = "";
    public decimal Price { get; set; }
    public string Tone { get; set; } = "tomato";
    public bool Featured { get; set; }
    public string Tags { get; set; } = "klasika";
    public int SortOrder { get; set; }
}
