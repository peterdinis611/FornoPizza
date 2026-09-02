namespace Forno.Configuration;

public sealed class AdminOptions
{
    public const string Section = "Admin";

    /// <summary>Ak je prázdne, admin je otvorený. Inak treba PIN.</summary>
    public string Pin { get; set; } = "";

    public bool RequiresPin => !string.IsNullOrWhiteSpace(Pin);
}
