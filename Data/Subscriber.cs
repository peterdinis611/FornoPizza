namespace Forno.Data;

public sealed class Subscriber
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; }
}
