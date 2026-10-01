using StatiCSharp.Interfaces;
using Xunit;

namespace StatiCSharp.Tests;

public class DefaultHtmlFactoryEscapingTests
{
    private static Website AWebsite(string name = "My Website") =>
        Website.Create(url: "https://example.com", name: name);

    private static RenderContext AContext(string name = "My Website") =>
        new() { Website = AWebsite(name) };

    [Fact]
    public void AnItemTitleIsEncodedInTheItemList()
    {
        List<IItem> items = [new Item { Title = "The \"quoted\" & <bold> post", Section = "posts" }];

        string html = new DefaultHtmlFactory().MakeTagListHtml(items, "a-tag", AContext());

        Assert.Contains("The &quot;quoted&quot; &amp; &lt;bold&gt; post", html, StringComparison.Ordinal);
        Assert.DoesNotContain("<bold>", html, StringComparison.Ordinal);
    }

    [Fact]
    public void AnItemDescriptionIsEncodedInTheItemList()
    {
        List<IItem> items = [new Item { Description = "Uses <script> & \"quotes\"", Section = "posts" }];

        string html = new DefaultHtmlFactory().MakeTagListHtml(items, "a-tag", AContext());

        Assert.Contains("Uses &lt;script&gt; &amp; &quot;quotes&quot;", html, StringComparison.Ordinal);
        Assert.DoesNotContain("<script>", html, StringComparison.Ordinal);
    }

    [Fact]
    public void ATagNameIsEncodedInTheHeadingAndInTheTagList()
    {
        List<IItem> items = [new Item { Section = "posts", Tags = ["a&b"] }];

        string html = new DefaultHtmlFactory().MakeTagListHtml(items, "c<d", AContext());

        // The heading renders the tag being listed, the item renders its own tags.
        Assert.Contains("c&lt;d", html, StringComparison.Ordinal);
        Assert.Contains("a&amp;b", html, StringComparison.Ordinal);
    }

    [Fact]
    public void TheWebsiteNameIsEncodedInTheSiteHeader()
    {
        string html = new DefaultHtmlFactory().MakePageHtml(new Page(), AContext("Roland's \"Blog\""));

        Assert.Contains("Roland&#39;s &quot;Blog&quot;", html, StringComparison.Ordinal);
    }

    [Fact]
    public void ASectionNameIsEncodedInTheNavigation()
    {
        // The fallback path: a section whose index.md carries no title is linked by its folder
        // name, so that name is what reaches the html unencoded if nobody encodes it.
        var context = new RenderContext
        {
            Website = AWebsite(),
            Sections = [new Section { SectionName = "a&b" }],
        };

        string html = new DefaultHtmlFactory().MakePageHtml(new Page(), context);

        Assert.Contains("a&amp;b", html, StringComparison.Ordinal);
    }

    [Fact]
    public void ATagLinkPointsAtTheSlugTheGeneratorWrites()
    {
        // The link used to interpolate the tag name straight into the path, so a tag with
        // a space produced /tag/web dev and a tag with a slash nested the directory.
        List<IItem> items = [new Item { Section = "posts", Tags = ["Web Dev"] }];

        string html = new DefaultHtmlFactory().MakeTagListHtml(items, "Web Dev", AContext());

        Assert.Contains("href=\"/tag/web-dev\"", html, StringComparison.Ordinal);
        Assert.DoesNotContain("/tag/Web Dev", html, StringComparison.Ordinal);
    }

    [Fact]
    public void ATagLinkShowsTheOriginalNameButLinksTheSlug()
    {
        List<IItem> items = [new Item { Section = "posts", Tags = ["Web Dev"] }];

        string html = new DefaultHtmlFactory().MakeTagListHtml(items, "a-tag", AContext());

        Assert.Contains("href=\"/tag/web-dev\">Web Dev</a>", html, StringComparison.Ordinal);
    }

    [Fact]
    public void TheSitesContentIsStillWrittenThroughAsHtml()
    {
        // Content is markdown rendered to html by the time it gets here. Encoding it
        // would print every page as visible source.
        var page = new Page { Content = "<p>Hello <em>world</em> &amp; welcome</p>" };

        string html = new DefaultHtmlFactory().MakePageHtml(page, AContext());

        Assert.Contains("<p>Hello <em>world</em> &amp; welcome</p>", html, StringComparison.Ordinal);
    }
}
