namespace Forno.Models;

public sealed record CartSnap(string Slug, int Quantity, string[]? Extras = null);
