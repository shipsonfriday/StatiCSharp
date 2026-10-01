using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace StatiCSharp.Tests;

/// <summary>
/// Covers generating more than once from the same website.
/// <para>
/// This used to go wrong in two ways at once: the writer kept its path bookkeeping between
/// runs, and reading appended to the website, so a second run saw every section and page
/// twice. Both are gone by construction now - one writer per run, and reading returns its
/// result instead of filling the website - so what is left to check is the outcome.
/// </para>
/// </summary>
public class RepeatedRunTests
{
    private static Website WriteSource(TempDirectory directory)
    {
        directory.WriteFile("Content/index.md", "---", "Title: Home", "---", "Welcome.");
        directory.WriteFile("Content/posts/index.md", "---", "Title: Posts", "---", "The section.");
        directory.WriteFile("Content/posts/a-post.md", "---", "Title: A Post", "Date: 2026-01-01", "Tags: one", "---", "Body.");
        directory.WriteFile("Content/about/index.md", "---", "Title: About", "---", "About.");
        Directory.CreateDirectory(Path.Combine(directory.Path, "Resources"));

        return Website.Create(url: "https://example.com", name: "My Website").WithSections("posts");
    }

    [Fact]
    public async Task TheOutputIsTheSameAfterASecondRun()
    {
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);
        string output = Path.Combine(directory.Path, "Output");

        await WebsiteManager.For(website, directory.Path).MakeAsync();
        string[] first = [.. Directory.GetFiles(output, "*", SearchOption.AllDirectories).Order()];

        await WebsiteManager.For(website, directory.Path).MakeAsync();
        string[] second = [.. Directory.GetFiles(output, "*", SearchOption.AllDirectories).Order()];

        Assert.Equal(first, second);
    }

    [Fact]
    public async Task TheHomepageListsEveryArticleOnceInEveryRun()
    {
        // The homepage lists the newest items across all sections. While reading appended to
        // the website, the second run listed every article twice - the kind of defect that
        // produces a plausible looking page rather than an error.
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);
        string index = Path.Combine(directory.Path, "Output", "index.html");

        await WebsiteManager.For(website, directory.Path).MakeAsync();
        string afterFirstRun = await File.ReadAllTextAsync(index, TestContext.Current.CancellationToken);

        await WebsiteManager.For(website, directory.Path).MakeAsync();
        string afterSecondRun = await File.ReadAllTextAsync(index, TestContext.Current.CancellationToken);

        Assert.Equal(afterFirstRun, afterSecondRun);
        Assert.Equal(1, Occurrences(afterFirstRun, "/posts/a-post"));
    }

    [Fact]
    public async Task ReusingOneManagerIsTheSameAsUsingTwo()
    {
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);
        string index = Path.Combine(directory.Path, "Output", "index.html");

        WebsiteManager manager = WebsiteManager.For(website, directory.Path);
        await manager.MakeAsync();
        string afterFirstRun = await File.ReadAllTextAsync(index, TestContext.Current.CancellationToken);

        await manager.MakeAsync();

        Assert.Equal(afterFirstRun, await File.ReadAllTextAsync(index, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task AnIndexFileDeletedBetweenRunsLeavesAnEmptyIndex()
    {
        // The index used to be an object on the website that reading filled in place, so
        // without an index.md the second run kept the title of the first.
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);
        string index = Path.Combine(directory.Path, "Output", "index.html");

        await WebsiteManager.For(website, directory.Path).MakeAsync();
        Assert.Contains("<title>Home</title>", await File.ReadAllTextAsync(index, TestContext.Current.CancellationToken), StringComparison.Ordinal);

        File.Delete(Path.Combine(directory.Path, "Content", "index.md"));
        await WebsiteManager.For(website, directory.Path).MakeAsync();

        Assert.Contains("<title></title>", await File.ReadAllTextAsync(index, TestContext.Current.CancellationToken), StringComparison.Ordinal);
    }

    /// <summary>
    /// How often the text contains the given string.
    /// </summary>
    private static int Occurrences(string text, string needle) => text.Split(needle).Length - 1;
}
