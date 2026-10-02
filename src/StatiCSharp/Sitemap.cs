using StatiCSharp.Interfaces;
using System.Globalization;
using System.Text;

namespace StatiCSharp;

/// <summary>
/// Builds the <c>sitemap.xml</c> of a website from the pages that were actually written.
/// <para>
/// Not derived from the content a second time, but read off what the run produced: every
/// <c>index.html</c> below the output directory is a page, and its directory is its url. A
/// sitemap built from the model could promise urls the generator decided not to write - a tag
/// whose name yields no url, an item that was skipped - while this one cannot.
/// </para>
/// <para>
/// No <c>lastmod</c>. The dates available here would be the wrong ones: only an item carries a
/// modification date from its file, and in a checkout - a build on CI, say - even that is the
/// time the file was cloned, not written. A sitemap of nothing but locations is valid, and one
/// claiming every page changed today is worse than one claiming nothing.
/// </para>
/// </summary>
internal static class Sitemap
{
    /// <summary>
    /// The name the file is written under, which crawlers and robots.txt expect.
    /// </summary>
    internal const string FileName = "sitemap.xml";

    /// <summary>
    /// Builds the sitemap.
    /// </summary>
    /// <param name="website">The website, whose url every entry is built on.</param>
    /// <param name="producedFiles">
    /// The files the run produced, relative to the output directory with forward slashes.
    /// </param>
    /// <returns>The content for <c>sitemap.xml</c>.</returns>
    /// <exception cref="ArgumentNullException">An argument is null.</exception>
    internal static string For(IWebsite website, IEnumerable<string> producedFiles)
    {
        ArgumentNullException.ThrowIfNull(website);
        ArgumentNullException.ThrowIfNull(producedFiles);

        // Sorted, because the order has to be the same on every run. Without that the file
        // changes although the website did not, and incremental output would rewrite it.
        IEnumerable<string> urls = producedFiles
            .Where(IsAPage)
            .Select(file => UrlOf(website.Url, file))
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal);

        StringBuilder sitemap = new();
        sitemap.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n");
        sitemap.Append("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">\n");

        foreach (string url in urls)
        {
            sitemap.Append(CultureInfo.InvariantCulture, $"  <url><loc>{Escaped(url)}</loc></url>\n");
        }

        sitemap.Append("</urlset>\n");

        return sitemap.ToString();
    }

    private static bool IsAPage(string file)
        => file.Equals("index.html", StringComparison.Ordinal)
        || file.EndsWith("/index.html", StringComparison.Ordinal);

    /// <summary>
    /// The absolute url a produced page is served at: its directory, since a page is written as
    /// the index.html of the directory it lives in.
    /// </summary>
    private static string UrlOf(string websiteUrl, string file)
    {
        string directory = file[..^"index.html".Length].TrimEnd('/');

        return directory.Length == 0 ? $"{websiteUrl}/" : $"{websiteUrl}/{directory}";
    }

    /// <summary>
    /// The five characters that have to be entity escaped in a sitemap. A url built by StatiC#
    /// contains none of them, but a page copied in from the resources directory may.
    /// </summary>
    private static string Escaped(string url) => url
        .Replace("&", "&amp;", StringComparison.Ordinal)
        .Replace("<", "&lt;", StringComparison.Ordinal)
        .Replace(">", "&gt;", StringComparison.Ordinal)
        .Replace("\"", "&quot;", StringComparison.Ordinal)
        .Replace("'", "&apos;", StringComparison.Ordinal);
}
