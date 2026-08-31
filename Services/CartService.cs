using Forno.Models;

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
        var ids = OvenExtras.Normalize(extras);
        var existing = _lines.FirstOrDefault(l =>
            l.Pizza.Slug == pizza.Slug && OvenExtras.Same(l.ExtraIds, ids));

        if (existing is null)
        {
            _lines.Add(new CartLine
            {
                Pizza = pizza,
                Quantity = quantity,
                ExtraIds = ids
            });
        }
        else
        {
            existing.Quantity += quantity;
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

        if (quantity <= 0)
        {
            _lines.Remove(line);
        }
        else
        {
            line.Quantity = quantity;
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
        _lines
            .Select(line => new CartSnap(
                line.Pizza.Slug,
                line.Quantity,
                line.ExtraIds.ToArray()))
            .ToList();

    public void Replace(IEnumerable<CartLine> lines)
    {
        _lines.Clear();
        _lines.AddRange(lines);
        Notify();
    }

    private void Notify() => Changed?.Invoke();
}
