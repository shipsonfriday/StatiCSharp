using System.Globalization;
using StatiCSharp.Interfaces;
using Xunit;

namespace StatiCSharp.Tests;

public class DefaultHtmlFactoryOrderingTests
{
    private static Item AnItem(string title, string date) => new()
    {
        Title = title,
        Section = "posts",
        MarkdownFileName = $"{title}.md",
        Date = DateOnly.Parse(date, CultureInfo.InvariantCulture),
    };

    /// <summary>
    /// The order in which the item titles appear in the rendered html.
    /// </summary>
    private static string[] TitleOrder(string html, params string[] titles)
        => [.. titles.Where(html.Contains).OrderBy(title => html.IndexOf(title, StringComparison.Ordinal))];

    [Fact]
    public void MakeSectionHtml_ListsItemsNewestFirst()
    {
        var section = new Section { SectionName = "posts" };
        section.Items.Add(AnItem("middle", "2025-01-01"));
        section.Items.Add(AnItem("newest", "2026-01-01"));
        section.Items.Add(AnItem("oldest", "2024-01-01"));

        Website website = Website.Create(url: "https://example.com", name: "My Website");
        string html = new DefaultHtmlFactory().MakeSectionHtml(section, new RenderContext { Website = website });

        Assert.Equal(["newest", "middle", "oldest"], TitleOrder(html, "newest", "middle", "oldest"));
    }

    [Fact]
    public void MakeSectionHtml_DoesNotReorderTheSectionsOwnList()
    {
        // Rendering used to sort and reverse section.Items in place, so the section's
        // collection came out of a render in a different order than it went in.
        var section = new Section { SectionName = "posts" };
        section.AddItem(AnItem("newest", "2026-01-01"));
        section.AddItem(AnItem("oldest", "2024-01-01"));

        string[] before = [.. section.Items.Select(item => item.Title)];

        Website website = Website.Create(url: "https://example.com", name: "My Website");
        new DefaultHtmlFactory().MakeSectionHtml(section, new RenderContext { Website = website });

        Assert.Equal(before, section.Items.Select(item => item.Title));
    }

    [Fact]
    public void MakeTagListHtml_DoesNotReorderTheGivenList()
    {
        List<IItem> items = [AnItem("newest", "2026-01-01"), AnItem("oldest", "2024-01-01")];
        string[] before = [.. items.Select(item => item.Title)];

        Website website = Website.Create(url: "https://example.com", name: "My Website");
        new DefaultHtmlFactory().MakeTagListHtml(items, "a-tag", new RenderContext { Website = website });

        Assert.Equal(before, items.Select(item => item.Title));
    }

    [Fact]
    public void MakeTagListHtml_ListsItemsNewestFirst()
    {
        List<IItem> items = [AnItem("oldest", "2024-01-01"), AnItem("newest", "2026-01-01")];

        Website website = Website.Create(url: "https://example.com", name: "My Website");
        string html = new DefaultHtmlFactory().MakeTagListHtml(items, "a-tag", new RenderContext { Website = website });

        Assert.Equal(["newest", "oldest"], TitleOrder(html, "newest", "oldest"));
    }

    [Fact]
    public void MakeIndexHtml_CollectsFromEverySectionNewestFirst()
    {
        var posts = new Section { SectionName = "posts" };
        posts.AddItem(AnItem("from-posts", "2025-06-01"));

        var notes = new Section { SectionName = "notes" };
        notes.AddItem(AnItem("from-notes", "2026-06-01"));

        var context = new RenderContext
        {
            Website = Website.Create(url: "https://example.com", name: "My Website"),
            Sections = [posts, notes],
        };

        string html = new DefaultHtmlFactory().MakeIndexHtml(new Index(), context);

        Assert.Equal(["from-notes", "from-posts"], TitleOrder(html, "from-notes", "from-posts"));
    }

    [Fact]
    public void MakeIndexHtml_ShowsAtMostTenItems()
    {
        var section = new Section { SectionName = "posts" };
        for (int day = 1; day <= 15; day++)
        {
            section.AddItem(AnItem($"post-{day:00}", $"2025-01-{day:00}"));
        }

        var context = new RenderContext
        {
            Website = Website.Create(url: "https://example.com", name: "My Website"),
            Sections = [section],
        };

        string html = new DefaultHtmlFactory().MakeIndexHtml(new Index(), context);

        // The ten newest are the 15th down to the 6th.
        Assert.Contains("post-15", html, StringComparison.Ordinal);
        Assert.Contains("post-06", html, StringComparison.Ordinal);
        Assert.DoesNotContain("post-05", html, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("de-DE")]
    [InlineData("en-US-POSIX")]
    public void Ordering_WorksInEveryCulture(string culture)
    {
        // The old comparison parsed the string "6pm" into a TimeOnly. Of the installed
        // cultures exactly one, en-US-POSIX, throws a FormatException on that.
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);

            List<IItem> items = [AnItem("oldest", "2024-01-01"), AnItem("newest", "2026-01-01")];
            Website website = Website.Create(url: "https://example.com", name: "My Website");

            string html = new DefaultHtmlFactory().MakeTagListHtml(items, "a-tag", new RenderContext { Website = website });

            Assert.Equal(["newest", "oldest"], TitleOrder(html, "newest", "oldest"));
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }
}
