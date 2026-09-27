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
}
