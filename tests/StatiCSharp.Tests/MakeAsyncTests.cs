using System.Globalization;
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
    public async Task ASecondRunKeepsEveryFile()
    {
        // Incremental output deletes what has no markdown equivalent, so a lost path entry
        // is even more destructive on a second run.
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);

        await WebsiteManager.For(website, directory.Path).MakeAsync();

        Website second = WriteSource(directory);
        await WebsiteManager.For(second, directory.Path).MakeAsync();

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

        await WebsiteManager.For(website, directory.Path).MakeAsync();

        string orphan = Path.Combine(directory.Path, "Output", "posts", "deleted-post");
        Directory.CreateDirectory(orphan);
        await File.WriteAllTextAsync(
            Path.Combine(orphan, "index.html"),
            "stale",
            TestContext.Current.CancellationToken);

        Website second = WriteSource(directory);
        await WebsiteManager.For(second, directory.Path).MakeAsync();

        Assert.False(Directory.Exists(orphan));
    }

    [Fact]
    public async Task AnUnchangedFileIsNotRewritten()
    {
        // The point of the default: a website under source control shows no change when no
        // content changed.
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);
        string index = Path.Combine(directory.Path, "Output", "index.html");

        await WebsiteManager.For(website, directory.Path).MakeAsync();

        DateTime marker = File.GetLastWriteTimeUtc(index).AddDays(-1);
        File.SetLastWriteTimeUtc(index, marker);
        await WebsiteManager.For(website, directory.Path).MakeAsync();

        Assert.Equal(marker, File.GetLastWriteTimeUtc(index));
    }

    [Fact]
    public async Task NoIncrementalOutputRewritesEveryFile()
    {
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);
        string index = Path.Combine(directory.Path, "Output", "index.html");

        await WebsiteManager.For(website, directory.Path).NoIncrementalOutput().MakeAsync();

        DateTime marker = File.GetLastWriteTimeUtc(index).AddDays(-1);
        File.SetLastWriteTimeUtc(index, marker);
        await WebsiteManager.For(website, directory.Path).NoIncrementalOutput().MakeAsync();

        Assert.NotEqual(marker, File.GetLastWriteTimeUtc(index));
    }

    [Fact]
    public async Task NoIncrementalOutputEmptiesTheOutputDirectoryFirst()
    {
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);

        await WebsiteManager.For(website, directory.Path).MakeAsync();

        string leftover = Path.Combine(directory.Path, "Output", "old-photo.png");
        await File.WriteAllTextAsync(leftover, "image", TestContext.Current.CancellationToken);

        await WebsiteManager.For(website, directory.Path).NoIncrementalOutput().MakeAsync();

        Assert.False(File.Exists(leftover));
    }

    [Fact]
    public async Task ADeletedResourceDisappearsFromTheOutput()
    {
        // The bookkeeping used to be per directory and removed nothing but index.html,
        // because any other file might have been a resource. A resource deleted from the
        // Resources directory therefore stayed in the output for good.
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);
        directory.WriteFile("Resources/old-logo.png", "image");

        await WebsiteManager.For(website, directory.Path).MakeAsync();
        string copied = Path.Combine(directory.Path, "Output", "old-logo.png");
        Assert.True(File.Exists(copied));

        File.Delete(Path.Combine(directory.Path, "Resources", "old-logo.png"));
        await WebsiteManager.For(website, directory.Path).MakeAsync();

        Assert.False(File.Exists(copied));
    }

    [Fact]
    public async Task AResourceThatIsStillThereSurvivesTheCleanUp()
    {
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);
        directory.WriteFile("Resources/logo.png", "image");

        await WebsiteManager.For(website, directory.Path).MakeAsync();
        await WebsiteManager.For(website, directory.Path).MakeAsync();

        Assert.True(File.Exists(Path.Combine(directory.Path, "Output", "logo.png")));
        Assert.True(File.Exists(Path.Combine(directory.Path, "Output", "favicon.png")));
    }

    [Fact]
    public async Task TheRepositoryInTheOutputDirectorySurvivesBothModes()
    {
        // An output directory is often the repository it is deployed from, so .git lives
        // inside it. Deleting that is the one unrecoverable thing the generator could do.
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);
        directory.WriteFile("Output/.git/HEAD", "ref: refs/heads/main");
        directory.WriteFile("Output/CNAME", "example.com");
        string head = Path.Combine(directory.Path, "Output", ".git", "HEAD");

        await WebsiteManager.For(website, directory.Path).MakeAsync();
        Assert.True(File.Exists(head));

        await WebsiteManager.For(website, directory.Path).NoIncrementalOutput().MakeAsync();
        Assert.True(File.Exists(head));
        Assert.True(File.Exists(Path.Combine(directory.Path, "Output", "CNAME")));
    }

    [Fact]
    public async Task AnUnchangedResourceIsNotRewrittenEither()
    {
        // The incremental part used to cover the generated html only. Resources were copied
        // over themselves on every run, so every image in the output looked modified.
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);
        directory.WriteFile("Resources/logo.png", "image");

        await WebsiteManager.For(website, directory.Path).MakeAsync();

        string[] copies =
        [
            Path.Combine(directory.Path, "Output", "logo.png"),
            Path.Combine(directory.Path, "Output", "favicon.png"),
            Path.Combine(directory.Path, "Output", "default-theme", "styles.css"),
        ];

        DateTime marker = DateTime.UtcNow.AddDays(-1);
        foreach (string copy in copies)
        {
            File.SetLastWriteTimeUtc(copy, marker);
        }

        await WebsiteManager.For(website, directory.Path).MakeAsync();

        foreach (string copy in copies)
        {
            Assert.Equal(marker, File.GetLastWriteTimeUtc(copy));
        }
    }

    [Fact]
    public async Task AResourcesDirectoryHoldingTheOutputFailsBeforeAnythingIsWritten()
    {
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => WebsiteManager.For(website, directory.Path)
                .WithResourcesDirectory(directory.Path)
                .MakeAsync());

        Assert.False(Directory.Exists(Path.Combine(directory.Path, "Output")));
    }

    [Fact]
    public async Task NothingIsWrittenOutsideTheOutputDirectory()
    {
        // A path entry in the front matter used to be used as given, so "../../escaped" wrote
        // the file next to the Output folder - where the clean up never looks, so it stayed
        // for good. With enough of them a markdown file could write anywhere.
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);
        directory.WriteFile(
            "Content/posts/escaper.md",
            "---", "title: Escaper", "path: ../../escaped", "---", "Body.");

        await WebsiteManager.For(website, directory.Path).MakeAsync();

        string[] outsideTheOutput =
        [
            .. Directory.GetFiles(directory.Path, "*.html", SearchOption.AllDirectories)
                .Where(file => !file.StartsWith(Path.Combine(directory.Path, "Output"), StringComparison.Ordinal))
        ];

        Assert.Empty(outsideTheOutput);
        Assert.True(File.Exists(Path.Combine(directory.Path, "Output", "posts", "escaped", "index.html")));
    }

    [Fact]
    public async Task AFolderWithSpacesAndCapitalsBecomesASluggedUrl()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Content/index.md", "---", "title: Home", "---", "Welcome.");
        directory.WriteFile("Content/My Section/index.md", "---", "title: Section", "---", "Body.");
        directory.WriteFile("Content/My Section/A Post.md", "---", "title: A Post", "---", "Body.");
        directory.WriteFile("Content/My Docs/A Page.md", "---", "title: Page", "---", "Body.");
        Directory.CreateDirectory(Path.Combine(directory.Path, "Resources"));

        Website website = Website.Create(url: "https://example.com", name: "My Website")
            .WithSections("My Section");

        await WebsiteManager.For(website, directory.Path).MakeAsync();

        string output = Path.Combine(directory.Path, "Output");

        Assert.True(File.Exists(Path.Combine(output, "my-section", "index.html")));
        Assert.True(File.Exists(Path.Combine(output, "my-section", "a-post", "index.html")));
        Assert.True(File.Exists(Path.Combine(output, "my-docs", "a-page", "index.html")));
    }

    [Fact]
    public async Task EveryUrlInTheOutputHasAFileBehindIt()
    {
        // The url and the output path used to be two calculations kept in step by a comment.
        // This walks every link the theme produced for a site of its own and checks there is
        // something there.
        using var directory = new TempDirectory();
        directory.WriteFile("Content/index.md", "---", "title: Home", "---", "Welcome.");
        directory.WriteFile("Content/My Section/index.md", "---", "title: Section", "---", "Body.");
        directory.WriteFile("Content/My Section/A Post.md", "---", "title: A Post", "tags: Web Dev", "---", "Body.");
        directory.WriteFile("Content/My Section/custom.md", "---", "title: Custom", "path: deep/inside", "---", "Body.");
        Directory.CreateDirectory(Path.Combine(directory.Path, "Resources"));

        Website website = Website.Create(url: "https://example.com", name: "My Website")
            .WithSections("My Section");

        var reader = new ContentReader(Path.Combine(directory.Path, "Content"), new HtmlBuilder(useDefaultMarkdownParser: true));
        RenderContext content = reader.Read(website);

        await WebsiteManager.For(website, directory.Path).MakeAsync();

        string output = Path.Combine(directory.Path, "Output");
        List<string> urls = [content.Index.Url];
        urls.AddRange(content.Pages.Select(page => page.Url));
        urls.AddRange(content.Sections.Select(section => section.Url));
        urls.AddRange(content.Sections.SelectMany(section => section.Items).Select(item => item.Url));

        foreach (string url in urls)
        {
            string file = Path.Combine([output, .. url.Split('/', StringSplitOptions.RemoveEmptyEntries), "index.html"]);

            Assert.True(File.Exists(file), $"{url} has no file at {file}");
        }
    }

    [Fact]
    public async Task WithPreservedOutputKeepsAHandWrittenFile()
    {
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);
        directory.WriteFile("Output/robots.txt", "User-agent: *");
        directory.WriteFile("Output/leftover.txt", "not mine");

        await WebsiteManager.For(website, directory.Path)
            .WithPreservedOutput("robots.txt")
            .MakeAsync();

        Assert.True(File.Exists(Path.Combine(directory.Path, "Output", "robots.txt")));
        Assert.False(File.Exists(Path.Combine(directory.Path, "Output", "leftover.txt")));
    }
}
