using Forno.Data;
using Microsoft.EntityFrameworkCore;

namespace Forno.Services;

public sealed class SubscriberService(IDbContextFactory<FornoDbContext> factory)
{
    public async Task<bool> AddAsync(string email, CancellationToken cancellation = default)
    {
        var trimmed = email.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(trimmed) || trimmed.Length > 120)
        {
            return false;
        }

        await using var db = await factory.CreateDbContextAsync(cancellation);

        if (await db.Subscribers.AnyAsync(s => s.Email == trimmed, cancellation))
        {
            return true;
        }

        db.Subscribers.Add(new Subscriber
        {
            Email = trimmed,
            CreatedAt = DateTimeOffset.UtcNow,
        });

        try
        {
            await db.SaveChangesAsync(cancellation);
        }
        catch (DbUpdateException)
        {
            return true;
        }

        return true;
    }
}
