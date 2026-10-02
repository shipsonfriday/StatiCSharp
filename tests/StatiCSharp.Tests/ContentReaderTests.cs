using System.Globalization;
using StatiCSharp.Interfaces;
using Xunit;

namespace StatiCSharp.Tests;

public class ContentReaderTests
{
    private static readonly DateOnly FileDate = new(2020, 3, 4);

    private static ContentReader ReaderFor(TempDirectory directory) => new(
        Path.Combine(directory.Path, "Content"),
        new HtmlBuilder(useDefaultMarkdownParser: true),
        _ => { });

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

        RenderContext content = ReaderFor(directory).Read(website);

        return Assert.Single(Assert.Single(content.Sections).Items);
    }

    /// <summary>
    /// Reads a content directory holding nothing but pages.
    /// </summary>
    private static IReadOnlyList<IPage> PagesIn(TempDirectory directory)
        => ReaderFor(directory)
            .Read(Website.Create(url: "https://example.com", name: "My Website"))
            .Pages;

    [Fact]
    public void APageInAFolderTakesThatFolderAsItsHierarchy()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Content/about/index.md", "---", "title: About", "---", "About me.");

        IPage page = Assert.Single(PagesIn(directory));

        Assert.Equal("about", page.Hierarchy);
        Assert.Equal("/about", page.Url);
    }

    [Fact]
    public void ANestedPageKeepsEveryFolderOnTheWay()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Content/docs/guide/setup.md", "---", "title: Setup", "---", "Body.");

        IPage page = Assert.Single(PagesIn(directory));

        Assert.Equal(Path.Combine("docs", "guide"), page.Hierarchy);
        Assert.Equal("/docs/guide/setup", page.Url);
    }

    /// <summary>
    /// Writes a section with an index file and returns the sections that were read.
    /// </summary>
    private static IReadOnlyList<ISection> SectionsFor(TempDirectory directory, params string[] declared)
    {
        foreach (string name in declared.Distinct())
        {
            directory.WriteFile($"Content/{name}/index.md", "---", $"title: {name} section", "---", "Body.");
        }

        return ReaderFor(directory)
            .Read(Website.Create(url: "https://example.com", name: "My Website").WithSections(declared))
            .Sections;
    }

    [Fact]
    public void SectionsComeInTheOrderTheyWereDeclared()
    {
        // Not in the order the file system returns the directories, which is arbitrary and
        // differs between platforms. A navigation built from these is then the one that was
        // asked for.
        using var directory = new TempDirectory();

        IReadOnlyList<ISection> sections = SectionsFor(directory, "zeta", "alpha", "middle");

        Assert.Equal(["zeta", "alpha", "middle"], sections.Select(section => section.SectionName));
    }

    [Fact]
    public void ASectionNamedTwiceIsReadOnce()
    {
        using var directory = new TempDirectory();

        IReadOnlyList<ISection> sections = SectionsFor(directory, "posts", "posts");

        Assert.Single(sections);
    }

    [Fact]
    public void ASectionDeclaredWithoutAFolderIsSkipped()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Content/posts/index.md", "---", "title: Posts", "---", "Body.");

        IReadOnlyList<ISection> sections = ReaderFor(directory)
            .Read(Website.Create(url: "https://example.com", name: "My Website")
                .WithSections("posts", "no-such-folder"))
            .Sections;

        Assert.Equal(["posts"], sections.Select(section => section.SectionName));
    }

    [Fact]
    public void ASectionNameThatDiffersInCaseIsNotASection()
    {
        // The folder is read as pages, so it must not be read as a section as well - on a
        // case insensitive file system that would have produced both from the same files.
        using var directory = new TempDirectory();
        directory.WriteFile("Content/posts/index.md", "---", "title: Posts", "---", "Body.");
        directory.WriteFile("Content/posts/a-post.md", "---", "title: A Post", "---", "Body.");

        RenderContext content = ReaderFor(directory)
            .Read(Website.Create(url: "https://example.com", name: "My Website").WithSections("Posts"));

        Assert.Empty(content.Sections);
        Assert.Equal(2, content.Pages.Count);
    }

    [Fact]
    public void ASectionFolderWithoutUsableCharactersIsSkipped()
    {
        // Its url would be "/", so the section would be written over the index of the website.
        using var directory = new TempDirectory();
        directory.WriteFile("Content/index.md", "---", "title: Home", "---", "Welcome.");
        directory.WriteFile("Content/+++/index.md", "---", "title: Nameless", "---", "Body.");

        RenderContext content = ReaderFor(directory)
            .Read(Website.Create(url: "https://example.com", name: "My Website").WithSections("+++"));

        Assert.Empty(content.Sections);
        Assert.Equal("Home", content.Index.Title);
    }

    [Fact]
    public void AnItemWithoutUsableCharactersInItsNameIsSkipped()
    {
        // It would have no segment of its own and be written over the section's own page.
        using var directory = new TempDirectory();
        directory.WriteFile("Content/posts/index.md", "---", "title: Posts", "---", "Body.");
        directory.WriteFile("Content/posts/+++.md", "---", "title: Nameless", "---", "Body.");
        directory.WriteFile("Content/posts/a-post.md", "---", "title: A Post", "---", "Body.");

        RenderContext content = ReaderFor(directory)
            .Read(Website.Create(url: "https://example.com", name: "My Website").WithSections("posts"));

        Assert.Equal(["A Post"], Assert.Single(content.Sections).Items.Select(item => item.Title));
    }

    [Fact]
    public void APageNamedIndexKeepsNoSegmentOfItsOwn()
    {
        // The one case where having no segment is right: about/index.md is the page of the
        // about folder, so it must not be mistaken for a nameless page and skipped.
        using var directory = new TempDirectory();
        directory.WriteFile("Content/about/index.md", "---", "title: About", "---", "Body.");

        IPage page = Assert.Single(PagesIn(directory));

        Assert.Equal("/about", page.Url);
    }

    [Fact]
    public void AnEscapingPathStaysInsideItsSection()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Content/posts/index.md", "---", "title: Posts", "---", "Body.");
        directory.WriteFile("Content/posts/escaper.md", "---", "title: Escaper", "path: ../../escaped", "---", "Body.");

        RenderContext content = ReaderFor(directory)
            .Read(Website.Create(url: "https://example.com", name: "My Website").WithSections("posts"));

        Assert.Equal("/posts/escaped", Assert.Single(Assert.Single(content.Sections).Items).Url);
    }

    [Fact]
    public void ReadingTwiceReturnsIndependentContent()
    {
        // The reader used to fill the website that was passed in, so a second run appended to
        // what the first had left there - every section and page once per run - and an index
        // kept the values of the previous run when there was no index.md. Nothing is shared
        // between two results now, so there is nothing to carry over.
        using var directory = new TempDirectory();
        directory.WriteFile("Content/index.md", "---", "title: Home", "---", "Welcome.");
        directory.WriteFile("Content/about/index.md", "---", "title: About", "---", "About.");
        directory.WriteFile("Content/posts/index.md", "---", "title: Posts", "---", "Section.");

        Website website = Website.Create(url: "https://example.com", name: "My Website")
            .WithSections("posts");
        ContentReader reader = ReaderFor(directory);

        RenderContext first = reader.Read(website);
        RenderContext second = reader.Read(website);

        Assert.Single(first.Sections);
        Assert.Single(second.Sections);
        Assert.Single(first.Pages);
        Assert.Single(second.Pages);
        Assert.NotSame(first.Index, second.Index);
        Assert.NotSame(first.Sections, second.Sections);
    }

    [Fact]
    public void WithoutAnIndexFileTheIndexIsEmpty()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Content/about/index.md", "---", "title: About", "---", "About.");

        RenderContext content = ReaderFor(directory)
            .Read(Website.Create(url: "https://example.com", name: "My Website"));

        Assert.Equal(string.Empty, content.Index.Title);
        Assert.Equal(string.Empty, content.Index.Content);
    }

    [Fact]
    public void RejectsAnUnusableContentDirectory()
    {
        var builder = new HtmlBuilder(useDefaultMarkdownParser: true);

        Assert.Throws<ArgumentNullException>(() => new ContentReader(null!, builder, _ => { }));
        Assert.Throws<ArgumentException>(() => new ContentReader("   ", builder, _ => { }));
        Assert.Throws<ArgumentNullException>(() => new ContentReader("/content", null!, _ => { }));
    }

    [Fact]
    public void RejectsAMissingWebsite()
    {
        using var directory = new TempDirectory();

        Assert.Throws<ArgumentNullException>(() => ReaderFor(directory).Read(null!));
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

        RenderContext content = ReaderFor(directory).Read(website);

        return [.. Assert.Single(content.Sections).Items.Select(item => item.MarkdownFileName).Order()];
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
