using System.Text.RegularExpressions;
using StatiCSharp.Interfaces;
using Xunit;

namespace StatiCSharp.Tests;

public class DefaultHtmlFactoryNavigationTests
{
    private static Section ASection(string name, string title)
        => new() { SectionName = name, Title = title };

    private static string NavigationOf(params ISection[] sections)
    {
        var context = new RenderContext
        {
            Website = Website.Create(url: "https://example.com", name: "My Website"),
            Sections = sections,
        };

        string html = new DefaultHtmlFactory().MakePageHtml(new Page(), context);

        Match nav = Regex.Match(html, "<nav>.*?</nav>", RegexOptions.Singleline);
        Assert.True(nav.Success, "The page has no nav element.");

        return nav.Value;
    }

    [Fact]
    public void TheNavigationHasExactlyOneList()
    {
        // The section list used to be built as a Ul and then wrapped in a second one, so
        // every page carried <nav><ul><ul><li>…</li></ul></ul></nav>. A ul may only contain
        // li elements, so that was invalid on every page of every generated site.
        string nav = NavigationOf(ASection("posts", "Posts"), ASection("notes", "Notes"));

        Assert.Equal(1, Regex.Count(nav, "<ul"));
        Assert.Equal(1, Regex.Count(nav, "</ul>"));
        Assert.DoesNotContain("<ul><ul>", nav, StringComparison.Ordinal);
    }

    [Fact]
    public void ALinkSaysTheSectionsTitleAndPointsAtItsUrl()
    {
        // It used to say the folder name, because the navigation was built from the configured
        // names and the theme had no way to reach the sections that were read.
        string nav = NavigationOf(ASection("posts", "Everything I wrote"));

        Assert.Equal(
            "<nav><ul><li><a href=\"/posts\">Everything I wrote</a></li></ul></nav>",
            nav);
    }

    [Fact]
    public void WithoutATitleTheLinkFallsBackToTheFolderName()
    {
        // An index.md without a title would otherwise produce an empty, unclickable link.
        string nav = NavigationOf(ASection("posts", string.Empty));

        Assert.Equal("<nav><ul><li><a href=\"/posts\">posts</a></li></ul></nav>", nav);
    }

    [Fact]
    public void TheLinksFollowTheOrderOfTheSections()
    {
        string nav = NavigationOf(ASection("zeta", "Zeta"), ASection("alpha", "Alpha"));

        Assert.Equal(
            "<nav><ul><li><a href=\"/zeta\">Zeta</a></li><li><a href=\"/alpha\">Alpha</a></li></ul></nav>",
            nav);
    }

    [Fact]
    public void ATitleIsEncoded()
    {
        string nav = NavigationOf(ASection("posts", "Roland's \"Posts\" & more"));

        Assert.Contains(
            "Roland&#39;s &quot;Posts&quot; &amp; more",
            nav,
            StringComparison.Ordinal);
    }

    [Fact]
    public void AWebsiteWithoutSectionsStillRendersAValidEmptyList()
    {
        string nav = NavigationOf();

        Assert.Equal("<nav><ul></ul></nav>", nav);
    }

    [Fact]
    public void ASectionWithoutAnIndexFileIsNotLinked()
    {
        // The reader only returns sections that have an index.md, so a section named in the
        // configuration without one is simply not there - it used to get a link to a page
        // that was never written.
        using var directory = new TempDirectory();
        directory.WriteFile("Content/posts/index.md", "---", "title: Posts", "---", "Body.");
        directory.WriteFile("Content/drafts/a-draft.md", "---", "title: A Draft", "---", "Body.");

        RenderContext content = new ContentReader(
                System.IO.Path.Combine(directory.Path, "Content"),
                new HtmlBuilder(useDefaultMarkdownParser: true))
            .Read(Website.Create(url: "https://example.com", name: "My Website")
                .WithSections("posts", "drafts"));

        string nav = NavigationOf([.. content.Sections]);

        Assert.DoesNotContain("/drafts", nav, StringComparison.Ordinal);
        Assert.Contains("/posts", nav, StringComparison.Ordinal);
    }
}
