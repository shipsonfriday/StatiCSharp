using System;
using System.Globalization;
using System.IO;
using System.Linq;
using StatiCSharp.Interfaces;
using Xunit;

namespace StatiCSharp.Tests;

public class ContentReaderTests
{
    private static readonly DateOnly FileDate = new(2020, 3, 4);

    private static ContentReader ReaderFor(TempDirectory directory) => new(
        Path.Combine(directory.Path, "Content"),
        new HtmlBuilder(useDefaultMarkdownParser: true));

    /// <summary>
    /// Builds a source directory with one section holding one item, gives the item file a
    /// known last-write time, and collects the markdown data from it.
    /// </summary>
    private static IItem LoadTheOnlyItem(TempDirectory directory, params string[] itemLines)
    {
        directory.WriteFile("Content/posts/index.md", "---", "title: Posts", "---", "The posts section.");
        string itemFile = directory.WriteFile("Content/posts/a-post.md", itemLines);
        File.SetLastWriteTime(itemFile, FileDate.ToDateTime(TimeOnly.MinValue));

        Website website = Website.Create(url: "https://example.com", name: "My Website")
            .WithSections("posts");

        ReaderFor(directory).ReadInto(website);

        return Assert.Single(Assert.Single(website.Sections).Items);
    }

    /// <summary>
    /// Reads a content directory holding nothing but pages, and returns the website.
    /// </summary>
    private static Website PagesIn(TempDirectory directory)
    {
        Website website = Website.Create(url: "https://example.com", name: "My Website");
        ReaderFor(directory).ReadInto(website);

        return website;
    }

    [Fact]
    public void APageInAFolderTakesThatFolderAsItsHierarchy()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Content/about/index.md", "---", "title: About", "---", "About me.");

        IPage page = Assert.Single(PagesIn(directory).Pages);

        Assert.Equal("about", page.Hierarchy);
        Assert.Equal("/about", page.Url);
    }

    [Fact]
    public void ANestedPageKeepsEveryFolderOnTheWay()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Content/docs/guide/setup.md", "---", "title: Setup", "---", "Body.");

        IPage page = Assert.Single(PagesIn(directory).Pages);

        Assert.Equal(Path.Combine("docs", "guide"), page.Hierarchy);
        Assert.Equal("/docs/guide/setup", page.Url);
    }

    [Fact]
    public void RejectsAnUnusableContentDirectory()
    {
        var builder = new HtmlBuilder(useDefaultMarkdownParser: true);

        Assert.Throws<ArgumentNullException>(() => new ContentReader(null!, builder));
        Assert.Throws<ArgumentException>(() => new ContentReader("   ", builder));
        Assert.Throws<ArgumentNullException>(() => new ContentReader("/content", null!));
    }

    [Fact]
    public void RejectsAMissingWebsite()
    {
        using var directory = new TempDirectory();

        Assert.Throws<ArgumentNullException>(() => ReaderFor(directory).ReadInto(null!));
    }

    [Fact]
    public void WithoutADate_TheItemFallsBackToTheFilesDate()
    {
        // The fallback used to read DateLastModified before it was assigned, so it picked
        // up the property default - today - instead of the date of the file.
        using var directory = new TempDirectory();

        IItem item = LoadTheOnlyItem(directory, "---", "title: A Post", "---", "Body.");

        Assert.Equal(FileDate, item.Date);
        Assert.Equal(FileDate, item.DateLastModified);
    }

    [Fact]
    public void WithAnEmptyDate_TheItemFallsBackToTheFilesDate()
    {
        using var directory = new TempDirectory();

        IItem item = LoadTheOnlyItem(directory, "---", "title: A Post", "date:", "---", "Body.");

        Assert.Equal(FileDate, item.Date);
    }

    [Fact]
    public void WithADate_TheMetaDataWins()
    {
        using var directory = new TempDirectory();

        IItem item = LoadTheOnlyItem(
            directory, "---", "title: A Post", "date: 2025-11-02", "---", "Body.");

        Assert.Equal(new DateOnly(2025, 11, 2), item.Date);
        Assert.Equal(FileDate, item.DateLastModified);
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("de-DE")]
    public void TheFallbackDoesNotDependOnTheCulture(string culture)
    {
        // The date used to be written into the meta data dictionary with ToString() and
        // read back again, which made the result depend on the machine's culture.
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);
            using var directory = new TempDirectory();

            IItem item = LoadTheOnlyItem(directory, "---", "title: A Post", "---", "Body.");

            Assert.Equal(FileDate, item.Date);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void TheSectionIndexFileDoesNotBecomeAnItem()
    {
        using var directory = new TempDirectory();

        IItem item = LoadTheOnlyItem(directory, "---", "title: A Post", "---", "Body.");

        Assert.Equal("a-post.md", item.MarkdownFileName);
    }

    /// <summary>
    /// Puts the given files into one section and returns the filenames that became items.
    /// </summary>
    private static string[] ItemFilenamesFor(TempDirectory directory, params string[] filenames)
    {
        directory.WriteFile("Content/posts/index.md", "---", "title: Posts", "---", "The posts section.");

        foreach (string filename in filenames)
        {
            directory.WriteFile($"Content/posts/{filename}", "---", $"title: {filename}", "---", "Body.");
        }

        Website website = Website.Create(url: "https://example.com", name: "My Website")
            .WithSections("posts");

        ReaderFor(directory).ReadInto(website);

        return [.. Assert.Single(website.Sections).Items.Select(item => item.MarkdownFileName).Order()];
    }

    [Fact]
    public void AFileWhoseNameEndsInIndexIsStillAnItem()
    {
        // The check used to look at the end of the whole path, so my-index.md matched
        // "index.md" and was skipped: it appeared in no list and was never written.
        using var directory = new TempDirectory();

        string[] items = ItemFilenamesFor(directory, "my-index.md", "a-post.md");

        Assert.Equal(["a-post.md", "my-index.md"], items);
    }

    [Fact]
    public void NonMarkdownFilesInASectionAreIgnored()
    {
        // There was no extension filter at all, so anything sitting in a section folder
        // was parsed as markdown. A .DS_Store even produced an item whose path segment
        // was empty, colliding with the section page itself.
        using var directory = new TempDirectory();
        directory.WriteFile("Content/posts/.DS_Store", "binary junk");
        directory.WriteFile("Content/posts/notes.txt", "not markdown");
        directory.WriteFile("Content/posts/photo.png", "not markdown either");

        string[] items = ItemFilenamesFor(directory, "a-post.md");

        Assert.Equal(["a-post.md"], items);
    }

    [Fact]
    public void AnUppercaseExtensionCountsAsMarkdown()
    {
        using var directory = new TempDirectory();

        string[] items = ItemFilenamesFor(directory, "A-Post.MD");

        Assert.Equal(["A-Post.MD"], items);
    }

}
