using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace StatiCSharp.Tests;

public class SitemapTests
{
    private static Website AWebsite(string url = "https://example.com")
        => Website.Create(url: url, name: "My Website");

    private static string[] LocationsIn(string sitemap)
        => [.. sitemap.Split('\n')
            .Where(line => line.Contains("<loc>", StringComparison.Ordinal))
            .Select(line => line.Split("<loc>")[1].Split("</loc>")[0])];

    [Fact]
    public void EveryPageBecomesAnAbsoluteUrl()
    {
        string sitemap = Sitemap.For(AWebsite(), ["index.html", "posts/index.html", "posts/a-post/index.html"]);

        Assert.Equal(
            ["https://example.com/", "https://example.com/posts", "https://example.com/posts/a-post"],
            LocationsIn(sitemap));
    }

    [Fact]
    public void OnlyPagesAreListed()
    {
        // Everything the run produced goes in, and most of it is not a page: style sheets,
        // images, fonts, the favicon.
        string sitemap = Sitemap.For(AWebsite(),
            ["index.html", "favicon.png", "default-theme/styles.css", "posts/index.html"]);

        Assert.Equal(["https://example.com/", "https://example.com/posts"], LocationsIn(sitemap));
    }

    [Fact]
    public void TheOrderIsTheSameWhateverOrderTheFilesArriveIn()
    {
        // The files come out of a concurrent dictionary, whose order is not defined. Without
        // sorting, the file would change although the website did not, and incremental output
        // would rewrite it on every run.
        string[] files = ["posts/b/index.html", "index.html", "posts/a/index.html"];

        Assert.Equal(
            Sitemap.For(AWebsite(), files),
            Sitemap.For(AWebsite(), [.. files.Reverse()]));
    }

    [Fact]
    public void ATrailingSlashOnTheWebsiteUrlDoesNotDoubleUp()
    {
        string sitemap = Sitemap.For(AWebsite("https://example.com/"), ["posts/index.html"]);

        Assert.Equal(["https://example.com/posts"], LocationsIn(sitemap));
    }

    [Fact]
    public void AUrlWithCharactersXmlCaresAboutIsEscaped()
    {
        // A url StatiC# builds never needs this - a slug has no such characters - but a page
        // copied in from the resources directory can.
        string sitemap = Sitemap.For(AWebsite(), ["a&b/index.html"]);

        Assert.Contains("<loc>https://example.com/a&amp;b</loc>", sitemap, StringComparison.Ordinal);
    }

    [Fact]
    public void AnEmptyWebsiteIsStillAValidSitemap()
    {
        string sitemap = Sitemap.For(AWebsite(), []);

        Assert.Contains("<urlset", sitemap, StringComparison.Ordinal);
        Assert.Contains("</urlset>", sitemap, StringComparison.Ordinal);
        Assert.Empty(LocationsIn(sitemap));
    }

    [Fact]
    public void RejectsNull()
    {
        Assert.Throws<ArgumentNullException>(() => Sitemap.For(null!, []));
        Assert.Throws<ArgumentNullException>(() => Sitemap.For(AWebsite(), null!));
    }

    [Fact]
    public async Task ItListsWhatWasWrittenAndNothingThatWasSkipped()
    {
        // The point of building it from the produced files: a sitemap derived from the content
        // would promise the tag page of "+++" and the item file with no usable name, neither of
        // which the generator writes.
        using var directory = new TempDirectory();
        directory.WriteFile("Content/index.md", "---", "title: Home", "---", "Welcome.");
        directory.WriteFile("Content/about/index.md", "---", "title: About", "---", "About.");
        directory.WriteFile("Content/posts/index.md", "---", "title: Posts", "---", "Section.");
        directory.WriteFile("Content/posts/a-post.md", "---", "title: A Post", "tags: Web Dev, +++", "---", "Body.");
        directory.WriteFile("Content/posts/+++.md", "---", "title: Nameless", "---", "Body.");
        Directory.CreateDirectory(Path.Combine(directory.Path, "Resources"));

        Website website = Website.Create(url: "https://example.com", name: "My Website")
            .WithSections("posts");

        await WebsiteManager.For(website, directory.Path).WithLog(_ => { }).MakeAsync();

        string output = Path.Combine(directory.Path, "Output");
        string[] locations = LocationsIn(
            await File.ReadAllTextAsync(Path.Combine(output, Sitemap.FileName), TestContext.Current.CancellationToken));

        Assert.Equal(
            [
                "https://example.com/",
                "https://example.com/about",
                "https://example.com/posts",
                "https://example.com/posts/a-post",
                "https://example.com/tag/web-dev",
            ],
            locations);

        // Every entry has a file behind it.
        foreach (string location in locations)
        {
            string relative = location["https://example.com/".Length..];
            string file = Path.Combine([output, .. relative.Split('/', StringSplitOptions.RemoveEmptyEntries), "index.html"]);

            Assert.True(File.Exists(file), $"{location} has no file at {file}");
        }
    }

    [Fact]
    public async Task TheSitemapIsNotRewrittenWhenNothingChanged()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Content/index.md", "---", "title: Home", "---", "Welcome.");
        Directory.CreateDirectory(Path.Combine(directory.Path, "Resources"));

        Website website = AWebsite();
        string file = Path.Combine(directory.Path, "Output", Sitemap.FileName);

        await WebsiteManager.For(website, directory.Path).WithLog(_ => { }).MakeAsync();

        DateTime marker = File.GetLastWriteTimeUtc(file).AddDays(-1);
        File.SetLastWriteTimeUtc(file, marker);
        await WebsiteManager.For(website, directory.Path).WithLog(_ => { }).MakeAsync();

        Assert.Equal(marker, File.GetLastWriteTimeUtc(file));
    }

    [Fact]
    public async Task TheSitemapSurvivesTheCleanUp()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Content/index.md", "---", "title: Home", "---", "Welcome.");
        Directory.CreateDirectory(Path.Combine(directory.Path, "Resources"));

        await WebsiteManager.For(AWebsite(), directory.Path).WithLog(_ => { }).MakeAsync();

        Assert.True(File.Exists(Path.Combine(directory.Path, "Output", Sitemap.FileName)));
    }
}
