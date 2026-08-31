using Forno.Models;

namespace Forno.Services;

public interface IMenuService
{
    Task<IReadOnlyList<PizzaItem>> AllAsync(CancellationToken cancellation = default);

    Task<IReadOnlyList<string>> SlugsAsync(CancellationToken cancellation = default);

    Task<IReadOnlyList<PizzaItem>> FeaturedAsync(CancellationToken cancellation = default);

    Task<PizzaItem?> FindAsync(string slug, CancellationToken cancellation = default);

    Task<IReadOnlyList<PizzaItem>> FilterAsync(string? query, string? tag, CancellationToken cancellation = default);
}
