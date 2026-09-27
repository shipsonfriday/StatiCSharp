using System;
using System.Collections.Generic;
using StatiCSharp.Interfaces;
using Xunit;

namespace StatiCSharp.Tests;

public class DefaultHtmlFactoryEscapingTests
{
    private static Website AWebsite(string name = "My Website") =>
        Website.Create(url: "https://example.com", name: name);

    [Fact]
    public void AnItemTitleIsEncodedInTheItemList()
    {
        List<IItem> items = [new Item { Title = "The \"quoted\" & <bold> post", Section = "posts" }];

        string html = new DefaultHtmlFactory(AWebsite()).MakeTagListHtml(items, "a-tag");

        Assert.Contains("The &quot;quoted&quot; &amp; &lt;bold&gt; post", html, StringComparison.Ordinal);
        Assert.DoesNotContain("<bold>", html, StringComparison.Ordinal);
    }

    [Fact]
    public void AnItemDescriptionIsEncodedInTheItemList()
    {
        List<IItem> items = [new Item { Description = "Uses <script> & \"quotes\"", Section = "posts" }];

        string html = new DefaultHtmlFactory(AWebsite()).MakeTagListHtml(items, "a-tag");

        Assert.Contains("Uses &lt;script&gt; &amp; &quot;quotes&quot;", html, StringComparison.Ordinal);
        Assert.DoesNotContain("<script>", html, StringComparison.Ordinal);
    }

    [Fact]
    public void ATagNameIsEncodedInTheHeadingAndInTheTagList()
    {
        List<IItem> items = [new Item { Section = "posts", Tags = ["a&b"] }];

        string html = new DefaultHtmlFactory(AWebsite()).MakeTagListHtml(items, "c<d");

        // The heading renders the tag being listed, the item renders its own tags.
        Assert.Contains("c&lt;d", html, StringComparison.Ordinal);
        Assert.Contains("a&amp;b", html, StringComparison.Ordinal);
    }

    [Fact]
    public void TheWebsiteNameIsEncodedInTheSiteHeader()
    {
        string html = new DefaultHtmlFactory(AWebsite("Roland's \"Blog\"")).MakePageHtml(new Page());

        Assert.Contains("Roland&#39;s &quot;Blog&quot;", html, StringComparison.Ordinal);
    }

    [Fact]
    public void ASectionNameIsEncodedInTheNavigation()
    {
        Website website = AWebsite().WithSections("a&b");

        string html = new DefaultHtmlFactory(website).MakePageHtml(new Page());

        Assert.Contains("a&amp;b", html, StringComparison.Ordinal);
    }

    [Fact]
    public void TheSitesContentIsStillWrittenThroughAsHtml()
    {
        // Content is markdown rendered to html by the time it gets here. Encoding it
        // would print every page as visible source.
        var page = new Page { Content = "<p>Hello <em>world</em> &amp; welcome</p>" };

        string html = new DefaultHtmlFactory(AWebsite()).MakePageHtml(page);

        Assert.Contains("<p>Hello <em>world</em> &amp; welcome</p>", html, StringComparison.Ordinal);
    }
}
