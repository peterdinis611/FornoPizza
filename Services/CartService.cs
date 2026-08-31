using Forno.Mapping;
using Forno.Models;
using Forno.Validation;

namespace Forno.Services;

public sealed class CartService
{
    public event Action? Changed;

    private readonly List<CartLine> _lines = [];

    public IReadOnlyList<CartLine> Lines => _lines;

    public int Count => _lines.Sum(l => l.Quantity);

    public decimal Total => _lines.Sum(l => l.LineTotal);

    public bool IsEmpty => _lines.Count == 0;

    public void Add(PizzaItem pizza, int quantity = 1, IReadOnlyList<string>? extras = null)
    {
        var incoming = CartMapper.ToLine(pizza, quantity, extras);
        var existing = _lines.FirstOrDefault(l =>
            l.Pizza.Slug == incoming.Pizza.Slug && OvenExtras.Same(l.ExtraIds, incoming.ExtraIds));

        if (existing is null)
        {
            _lines.Add(incoming);
        }
        else
        {
            existing.Quantity = CartRules.ClampQty(existing.Quantity + incoming.Quantity);
        }

        Notify();
    }

    public void SetQuantity(string key, int quantity)
    {
        var line = _lines.FirstOrDefault(l => l.Key == key);
        if (line is null)
        {
            return;
        }

        if (quantity < 1)
        {
            _lines.Remove(line);
        }
        else
        {
            line.Quantity = CartRules.ClampQty(quantity);
        }

        Notify();
    }

    public void Remove(string key)
    {
        _lines.RemoveAll(l => l.Key == key);
        Notify();
    }

    public void Clear()
    {
        _lines.Clear();
        Notify();
    }

    public IReadOnlyList<CartSnap> Snapshot() =>
        _lines.Select(CartMapper.ToSnap).ToList();

    public void Replace(IEnumerable<CartLine> lines)
    {
        _lines.Clear();
        _lines.AddRange(lines.Select(CartMapper.Normalize));
        Notify();
    }

    private void Notify() => Changed?.Invoke();
}
