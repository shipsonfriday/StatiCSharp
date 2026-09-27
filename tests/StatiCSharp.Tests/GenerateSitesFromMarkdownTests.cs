using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StatiCSharp.Interfaces;
using Xunit;

namespace StatiCSharp.Tests;

public class GenerateSitesFromMarkdownTests
{
    private static readonly DateOnly FileDate = new(2020, 3, 4);

    /// <summary>
    /// Builds a source directory with one section holding one item, gives the item file a
    /// known last-write time, and collects the markdown data from it.
    /// </summary>
    private static async Task<IItem> LoadTheOnlyItemAsync(TempDirectory directory, params string[] itemLines)
    {
        directory.WriteFile("Content/posts/index.md", "---", "title: Posts", "---", "The posts section.");
        string itemFile = directory.WriteFile("Content/posts/a-post.md", itemLines);
        File.SetLastWriteTime(itemFile, FileDate.ToDateTime(TimeOnly.MinValue));

        Website website = Website.Create(url: "https://example.com", name: "My Website")
            .WithSections("posts");

        WebsiteManager manager = WebsiteManager.For(website, directory.Path);
        await manager.GenerateSitesFromMarkdownAsync();

        return Assert.Single(Assert.Single(website.Sections).Items);
    }

    [Fact]
    public async Task WithoutADate_TheItemFallsBackToTheFilesDate()
    {
        // The fallback used to read DateLastModified before it was assigned, so it picked
        // up the property default - today - instead of the date of the file.
        using var directory = new TempDirectory();

        IItem item = await LoadTheOnlyItemAsync(directory, "---", "title: A Post", "---", "Body.");

        Assert.Equal(FileDate, item.Date);
        Assert.Equal(FileDate, item.DateLastModified);
    }

    [Fact]
    public async Task WithAnEmptyDate_TheItemFallsBackToTheFilesDate()
    {
        using var directory = new TempDirectory();

        IItem item = await LoadTheOnlyItemAsync(directory, "---", "title: A Post", "date:", "---", "Body.");

        Assert.Equal(FileDate, item.Date);
    }

    [Fact]
    public async Task WithADate_TheMetaDataWins()
    {
        using var directory = new TempDirectory();

        IItem item = await LoadTheOnlyItemAsync(
            directory, "---", "title: A Post", "date: 2025-11-02", "---", "Body.");

        Assert.Equal(new DateOnly(2025, 11, 2), item.Date);
        Assert.Equal(FileDate, item.DateLastModified);
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("de-DE")]
    public async Task TheFallbackDoesNotDependOnTheCulture(string culture)
    {
        // The date used to be written into the meta data dictionary with ToString() and
        // read back again, which made the result depend on the machine's culture.
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);
            using var directory = new TempDirectory();

            IItem item = await LoadTheOnlyItemAsync(directory, "---", "title: A Post", "---", "Body.");

            Assert.Equal(FileDate, item.Date);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public async Task TheSectionIndexFileDoesNotBecomeAnItem()
    {
        using var directory = new TempDirectory();

        IItem item = await LoadTheOnlyItemAsync(directory, "---", "title: A Post", "---", "Body.");

        Assert.Equal("a-post.md", item.MarkdownFileName);
    }

    /// <summary>
    /// Puts the given files into one section and returns the filenames that became items.
    /// </summary>
    private static async Task<string[]> ItemFilenamesForAsync(TempDirectory directory, params string[] filenames)
    {
        directory.WriteFile("Content/posts/index.md", "---", "title: Posts", "---", "The posts section.");

        foreach (string filename in filenames)
        {
            directory.WriteFile($"Content/posts/{filename}", "---", $"title: {filename}", "---", "Body.");
        }

        Website website = Website.Create(url: "https://example.com", name: "My Website")
            .WithSections("posts");

        await WebsiteManager.For(website, directory.Path).GenerateSitesFromMarkdownAsync();

        return [.. Assert.Single(website.Sections).Items.Select(item => item.MarkdownFileName).Order()];
    }

    [Fact]
    public async Task AFileWhoseNameEndsInIndexIsStillAnItem()
    {
        // The check used to look at the end of the whole path, so my-index.md matched
        // "index.md" and was skipped: it appeared in no list and was never written.
        using var directory = new TempDirectory();

        string[] items = await ItemFilenamesForAsync(directory, "my-index.md", "a-post.md");

        Assert.Equal(["a-post.md", "my-index.md"], items);
    }

    [Fact]
    public async Task NonMarkdownFilesInASectionAreIgnored()
    {
        // There was no extension filter at all, so anything sitting in a section folder
        // was parsed as markdown. A .DS_Store even produced an item whose path segment
        // was empty, colliding with the section page itself.
        using var directory = new TempDirectory();
        directory.WriteFile("Content/posts/.DS_Store", "binary junk");
        directory.WriteFile("Content/posts/notes.txt", "not markdown");
        directory.WriteFile("Content/posts/photo.png", "not markdown either");

        string[] items = await ItemFilenamesForAsync(directory, "a-post.md");

        Assert.Equal(["a-post.md"], items);
    }

    [Fact]
    public async Task AnUppercaseExtensionCountsAsMarkdown()
    {
        using var directory = new TempDirectory();

        string[] items = await ItemFilenamesForAsync(directory, "A-Post.MD");

        Assert.Equal(["A-Post.MD"], items);
    }

}
