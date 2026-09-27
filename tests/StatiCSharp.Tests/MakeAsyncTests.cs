using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace StatiCSharp.Tests;

public class MakeAsyncTests
{
    private const int ItemCount = 150;

    /// <summary>
    /// Builds a source directory with one section holding <see cref="ItemCount"/> items,
    /// each with its own tag, plus a page and an index.
    /// </summary>
    private static Website WriteSource(TempDirectory directory)
    {
        directory.WriteFile("Content/index.md", "---", "title: Home", "---", "Welcome.");
        directory.WriteFile("Content/posts/index.md", "---", "title: Posts", "---", "The posts section.");
        directory.WriteFile("Content/about/index.md", "---", "title: About", "---", "About me.");

        for (int i = 1; i <= ItemCount; i++)
        {
            string number = i.ToString("000", CultureInfo.InvariantCulture);
            directory.WriteFile(
                $"Content/posts/post-{number}.md",
                "---",
                $"title: Post {number}",
                $"date: 2025-01-{(i % 28) + 1:00}",
                $"tags: tag-{number}, shared",
                "---",
                $"Body of post {number}.");
        }

        Directory.CreateDirectory(Path.Combine(directory.Path, "Resources"));

        return Website.Create(url: "https://example.com", name: "My Website")
            .WithSections("posts");
    }

    [Fact]
    public async Task EveryItemPageSurvivesTheRun()
    {
        // The paths written to were collected in a List that the Task.WhenAll tasks wrote
        // to concurrently. Entries lost that way make CleanUpAsync delete files that were
        // just written, so a run with many items is where it shows.
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);

        await WebsiteManager.For(website, directory.Path).MakeAsync();

        string output = Path.Combine(directory.Path, "Output");

        for (int i = 1; i <= ItemCount; i++)
        {
            string number = i.ToString("000", CultureInfo.InvariantCulture);
            Assert.True(
                File.Exists(Path.Combine(output, "posts", $"post-{number}", "index.html")),
                $"post-{number} is missing from the output");
        }
    }

    [Fact]
    public async Task EveryTagPageSurvivesTheRun()
    {
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);

        await WebsiteManager.For(website, directory.Path).MakeAsync();

        string tagDirectory = Path.Combine(directory.Path, "Output", "tag");

        for (int i = 1; i <= ItemCount; i++)
        {
            string number = i.ToString("000", CultureInfo.InvariantCulture);
            Assert.True(
                File.Exists(Path.Combine(tagDirectory, $"tag-{number}", "index.html")),
                $"tag-{number} is missing from the output");
        }

        Assert.True(File.Exists(Path.Combine(tagDirectory, "shared", "index.html")));
    }

    [Fact]
    public async Task IndexSectionAndPageAreWritten()
    {
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);

        await WebsiteManager.For(website, directory.Path).MakeAsync();

        string output = Path.Combine(directory.Path, "Output");

        Assert.True(File.Exists(Path.Combine(output, "index.html")));
        Assert.True(File.Exists(Path.Combine(output, "posts", "index.html")));
        Assert.True(File.Exists(Path.Combine(output, "about", "index.html")));
    }

    [Fact]
    public async Task TheThemesResourcesEndUpInTheOutput()
    {
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);

        await WebsiteManager.For(website, directory.Path).MakeAsync();

        string output = Path.Combine(directory.Path, "Output");

        Assert.True(File.Exists(Path.Combine(output, "favicon.png")));
        Assert.True(File.Exists(Path.Combine(output, "default-theme", "styles.css")));
    }

    [Fact]
    public async Task ASecondRunInGitModeKeepsEveryFile()
    {
        // GitMode also deletes output without a markdown equivalent, so a lost path entry
        // is even more destructive on a second run.
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);

        await WebsiteManager.For(website, directory.Path).WithGitMode().MakeAsync();

        Website second = WriteSource(directory);
        await WebsiteManager.For(second, directory.Path).WithGitMode().MakeAsync();

        string output = Path.Combine(directory.Path, "Output");
        int written = Directory.GetFiles(output, "index.html", SearchOption.AllDirectories).Length;

        // index + section + page + one per item + one per own tag + the shared tag
        Assert.Equal(3 + ItemCount + ItemCount + 1, written);
    }

    [Fact]
    public async Task OutputWithoutAMarkdownEquivalentIsRemoved()
    {
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);

        await WebsiteManager.For(website, directory.Path).WithGitMode().MakeAsync();

        string orphan = Path.Combine(directory.Path, "Output", "posts", "deleted-post");
        Directory.CreateDirectory(orphan);
        await File.WriteAllTextAsync(
            Path.Combine(orphan, "index.html"),
            "stale",
            TestContext.Current.CancellationToken);

        Website second = WriteSource(directory);
        await WebsiteManager.For(second, directory.Path).WithGitMode().MakeAsync();

        Assert.False(Directory.Exists(orphan));
    }
}
