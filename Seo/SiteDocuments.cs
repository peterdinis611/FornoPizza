using System.Globalization;
using System.Text;

namespace Forno.Seo;

public static class SiteDocuments
{
    public static string PublicBase(HttpRequest request, IConfiguration config)
    {
        var configured = config["SiteUrl"];
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return configured.TrimEnd('/');
        }

        var scheme = request.Headers["X-Forwarded-Proto"].FirstOrDefault() ?? request.Scheme;
        return $"{scheme}://{request.Host.Value}".TrimEnd('/');
    }

    public static string Robots(string baseUrl) =>
        $"""
        User-agent: *
        Allow: /

        Disallow: /kosik
        Disallow: /objednavka
        Disallow: /Error
        Disallow: /not-found
        Disallow: /kiln-status

        Sitemap: {baseUrl}/sitemap.xml

        """;

    public static string Sitemap(string baseUrl, IEnumerable<string> slugs)
    {
        var today = DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var xml = new StringBuilder();
        xml.AppendLine("""<?xml version="1.0" encoding="UTF-8"?>""");
        xml.AppendLine("""<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">""");

        WriteUrl(xml, baseUrl, "/", today, "daily", "1.0");
        WriteUrl(xml, baseUrl, "/menu", today, "weekly", "0.9");

        foreach (var slug in slugs)
        {
            WriteUrl(xml, baseUrl, $"/menu/{slug}", today, "weekly", "0.8");
        }

        xml.AppendLine("</urlset>");
        return xml.ToString();
    }

    private static void WriteUrl(
        StringBuilder xml,
        string baseUrl,
        string path,
        string lastmod,
        string changefreq,
        string priority)
    {
        xml.Append("  <url><loc>");
        xml.Append(System.Security.SecurityElement.Escape($"{baseUrl}{path}"));
        xml.Append("</loc><lastmod>");
        xml.Append(lastmod);
        xml.Append("</lastmod><changefreq>");
        xml.Append(changefreq);
        xml.Append("</changefreq><priority>");
        xml.Append(priority);
        xml.AppendLine("</priority></url>");
    }
}
