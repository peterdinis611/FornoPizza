using System.Globalization;
using Forno.Components;
using Forno.Data;
using Forno.Seo;
using Forno.Services;
using Microsoft.EntityFrameworkCore;

var sk = new CultureInfo("sk-SK");
CultureInfo.DefaultThreadCurrentCulture = sk;
CultureInfo.DefaultThreadCurrentUICulture = sk;

var builder = WebApplication.CreateBuilder(args);

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "Data", "forno.db");
Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

builder.Services.AddDbContextFactory<FornoDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<SubscriberService>();

var app = builder.Build();

await using (var db = await app.Services.GetRequiredService<IDbContextFactory<FornoDbContext>>().CreateDbContextAsync())
{
    await db.Database.MigrateAsync();
    await db.Database.ExecuteSqlRawAsync("PRAGMA journal_mode=WAL;");
    await FornoSeeder.SeedAsync(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/kiln-status/{0}", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapGet("/robots.txt", (HttpRequest request, IConfiguration config) =>
{
    var body = SiteDocuments.Robots(SiteDocuments.PublicBase(request, config));
    return Results.Text(body, "text/plain; charset=utf-8");
});

app.MapGet("/sitemap.xml", async (HttpRequest request, IConfiguration config, MenuService menu) =>
{
    var slugs = await menu.SlugsAsync();
    var body = SiteDocuments.Sitemap(SiteDocuments.PublicBase(request, config), slugs);
    return Results.Text(body, "application/xml; charset=utf-8");
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
