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

    public void Add(PizzaItem pizza, int quantity = 1)
    {
        var existing = _lines.FirstOrDefault(l => l.Pizza.Slug == pizza.Slug);
        if (existing is null)
        {
            _lines.Add(new CartLine { Pizza = pizza, Quantity = quantity });
        }
        else
        {
            existing.Quantity += quantity;
        }

        Notify();
    }

    public void SetQuantity(string slug, int quantity)
    {
        var line = _lines.FirstOrDefault(l => l.Pizza.Slug == slug);
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

    public void Remove(string slug)
    {
        _lines.RemoveAll(l => l.Pizza.Slug == slug);
        Notify();
    }

    public void Clear()
    {
        _lines.Clear();
        Notify();
    }

    private void Notify() => Changed?.Invoke();
}
