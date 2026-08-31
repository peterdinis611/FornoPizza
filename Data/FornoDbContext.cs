using Microsoft.EntityFrameworkCore;

namespace Forno.Data;

public sealed class FornoDbContext(DbContextOptions<FornoDbContext> options) : DbContext(options)
{
    public DbSet<Pizza> Pizzas => Set<Pizza>();
    public DbSet<OvenOrder> Orders => Set<OvenOrder>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();
    public DbSet<Subscriber> Subscribers => Set<Subscriber>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Pizza>(entity =>
        {
            entity.ToTable("pizzas");
            entity.HasIndex(p => p.Slug).IsUnique();
            entity.Property(p => p.Slug).HasMaxLength(80).IsRequired();
            entity.Property(p => p.Name).HasMaxLength(80).IsRequired();
            entity.Property(p => p.Tagline).HasMaxLength(120).IsRequired();
            entity.Property(p => p.Description).HasMaxLength(480).IsRequired();
            entity.Property(p => p.Ingredients).HasMaxLength(240).IsRequired();
            entity.Property(p => p.Tone).HasMaxLength(24).IsRequired();
            entity.Property(p => p.Tags).HasMaxLength(80).IsRequired();
            entity.Property(p => p.Price).HasPrecision(6, 2);
        });

        model.Entity<OvenOrder>(entity =>
        {
            entity.ToTable("orders");
            entity.Property(o => o.Name).HasMaxLength(80).IsRequired();
            entity.Property(o => o.Phone).HasMaxLength(30).IsRequired();
            entity.Property(o => o.Address).HasMaxLength(160).IsRequired();
            entity.Property(o => o.Note).HasMaxLength(240);
            entity.Property(o => o.Status).HasMaxLength(24).IsRequired();
            entity.Property(o => o.Total).HasPrecision(8, 2);
            entity.HasMany(o => o.Lines)
                .WithOne(l => l.Order)
                .HasForeignKey(l => l.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        model.Entity<OrderLine>(entity =>
        {
            entity.ToTable("order_lines");
            entity.Property(l => l.PizzaSlug).HasMaxLength(80).IsRequired();
            entity.Property(l => l.PizzaName).HasMaxLength(80).IsRequired();
            entity.Property(l => l.UnitPrice).HasPrecision(6, 2);
            entity.Ignore(l => l.LineTotal);
        });

        model.Entity<Subscriber>(entity =>
        {
            entity.ToTable("subscribers");
            entity.HasIndex(s => s.Email).IsUnique();
            entity.Property(s => s.Email).HasMaxLength(120).IsRequired();
        });
    }
}
