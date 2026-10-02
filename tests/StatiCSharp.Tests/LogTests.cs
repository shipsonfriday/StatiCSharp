using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace StatiCSharp.Tests;

/// <summary>
/// Covers what the generator reports and that a caller can take it.
/// <para>
/// Every message used to go straight to the console, so none of these warnings had a test:
/// nothing could observe them. They are the interesting output - a build script that wants to
/// fail on a broken date or a colliding url has to be able to see them.
/// </para>
/// </summary>
public class LogTests
{
    private static Website WriteSource(TempDirectory directory)
    {
        directory.WriteFile("Content/index.md", "---", "title: Home", "---", "Welcome.");
        directory.WriteFile("Content/posts/index.md", "---", "title: Posts", "---", "Section.");
        Directory.CreateDirectory(Path.Combine(directory.Path, "Resources"));

        return Website.Create(url: "https://example.com", name: "My Website").WithSections("posts");
    }

    private static async Task<RecordedLog> GenerateAsync(TempDirectory directory, Website website)
    {
        var log = new RecordedLog();
        await WebsiteManager.For(website, directory.Path).WithLog(log.Write).MakeAsync();

        return log;
    }

    [Fact]
    public async Task TheProgressIsReported()
    {
        using var directory = new TempDirectory();
        RecordedLog log = await GenerateAsync(directory, WriteSource(directory));

        Assert.Contains("Collecting markdown data...", log.Messages);
        Assert.Contains("Writing tag lists...", log.Messages);
        Assert.Contains(log.Messages, message => message.StartsWith("Success!", StringComparison.Ordinal));
    }

    [Fact]
    public async Task AQuietRunStillGeneratesTheSite()
    {
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);

        await WebsiteManager.For(website, directory.Path).WithLog(_ => { }).MakeAsync();

        Assert.True(File.Exists(Path.Combine(directory.Path, "Output", "index.html")));
    }

    [Fact]
    public async Task ADateThatCannotBeReadIsReportedWithItsFile()
    {
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);
        directory.WriteFile(
            "Content/posts/a-post.md",
            "---", "title: A Post", "date: last tuesday", "---", "Body.");

        RecordedLog log = await GenerateAsync(directory, website);

        Assert.True(log.Reported("WARNING:", "last tuesday", "a-post.md", "ISO 8601"),
            string.Join("\n", log.Warnings));
    }

    [Fact]
    public async Task ADuplicateKeyInTheFrontMatterIsReported()
    {
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);
        directory.WriteFile(
            "Content/posts/a-post.md",
            "---", "title: First", "title: Second", "---", "Body.");

        RecordedLog log = await GenerateAsync(directory, website);

        Assert.True(log.Reported("WARNING:", "\"title\"", "more than once"),
            string.Join("\n", log.Warnings));
    }

    [Fact]
    public async Task ALineWithoutAColonIsReported()
    {
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);
        directory.WriteFile(
            "Content/posts/a-post.md",
            "---", "title: A Post", "this line has no colon", "---", "Body.");

        RecordedLog log = await GenerateAsync(directory, website);

        Assert.True(log.Reported("WARNING:", "this line has no colon", "no colon"),
            string.Join("\n", log.Warnings));
    }

    [Fact]
    public async Task TwoTagsLeadingToOneUrlAreReported()
    {
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);
        directory.WriteFile(
            "Content/posts/a-post.md",
            "---", "title: A Post", "tags: Web Dev, web-dev", "---", "Body.");

        RecordedLog log = await GenerateAsync(directory, website);

        Assert.True(log.Reported("WARNING:", "/tag/web-dev", "Only one of them"),
            string.Join("\n", log.Warnings));
    }

    [Fact]
    public async Task TwoSitesWrittenToOneUrlAreReported()
    {
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);
        directory.WriteFile("Content/posts/one.md", "---", "title: One", "path: same", "---", "Body.");
        directory.WriteFile("Content/posts/two.md", "---", "title: Two", "path: same", "---", "Body.");

        RecordedLog log = await GenerateAsync(directory, website);

        Assert.True(log.Reported("WARNING:", "/posts/same"),
            string.Join("\n", log.Warnings));
    }

    [Fact]
    public async Task AFileThatCannotBecomeAUrlIsReported()
    {
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);
        directory.WriteFile("Content/posts/+++.md", "---", "title: Nameless", "---", "Body.");

        RecordedLog log = await GenerateAsync(directory, website);

        Assert.True(log.Reported("WARNING:", "+++.md", "Skipping it"),
            string.Join("\n", log.Warnings));
    }

    [Fact]
    public async Task ACleanRunReportsNoWarningAtAll()
    {
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);
        directory.WriteFile(
            "Content/posts/a-post.md",
            "---", "title: A Post", "date: 2026-01-01", "tags: one, two", "---", "Body.");

        RecordedLog log = await GenerateAsync(directory, website);

        Assert.Empty(log.Warnings);
    }

    [Fact]
    public void WithLogRejectsNull()
    {
        Website website = Website.Create(url: "https://example.com", name: "My Website");

        Assert.Throws<ArgumentNullException>(
            () => WebsiteManager.For(website, "source").WithLog(null!));
    }
}
