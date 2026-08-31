using Forno.Contracts;
using Forno.Data;
using Forno.Validation;
using Microsoft.EntityFrameworkCore;

namespace Forno.Services;

public sealed class SubscriberService(IDbContextFactory<FornoDbContext> factory) : ISubscriberService
{
    public async Task<Result> AddAsync(string email, CancellationToken cancellation = default)
    {
        var check = SubscribeValidator.Validate(email);
        if (!check.IsSuccess)
        {
            return check;
        }

        var trimmed = InputText.Email(email);
        await using var db = await factory.CreateDbContextAsync(cancellation);

        if (await db.Subscribers.AnyAsync(s => s.Email == trimmed, cancellation))
        {
            return Result.Ok();
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
            return Result.Ok();
        }

        return Result.Ok();
    }
}
