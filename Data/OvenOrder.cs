namespace Forno.Data;

public sealed class OvenOrder
{
    public int Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string Name { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Address { get; set; } = "";
    public string Note { get; set; } = "";
    public decimal Total { get; set; }
    public string Status { get; set; } = "prijata";

    public List<OrderLine> Lines { get; set; } = [];
}
