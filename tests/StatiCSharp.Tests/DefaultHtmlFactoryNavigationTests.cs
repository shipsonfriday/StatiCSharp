using System;
using System.Text.RegularExpressions;
using Xunit;

namespace StatiCSharp.Tests;

public class DefaultHtmlFactoryNavigationTests
{
    private static string NavigationOf(params string[] sections)
    {
        Website website = Website.Create(url: "https://example.com", name: "My Website")
            .WithSections(sections);

        string html = new DefaultHtmlFactory(website).MakePageHtml(new Page());

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
        string nav = NavigationOf("posts", "notes");

        Assert.Equal(1, Regex.Count(nav, "<ul"));
        Assert.Equal(1, Regex.Count(nav, "</ul>"));
        Assert.DoesNotContain("<ul><ul>", nav, StringComparison.Ordinal);
    }

    [Fact]
    public void EverySectionBecomesAListItemWithALink()
    {
        string nav = NavigationOf("posts", "notes");

        Assert.Equal(
            "<nav><ul><li><a href=\"/posts\">posts</a></li><li><a href=\"/notes\">notes</a></li></ul></nav>",
            nav);
    }

    [Fact]
    public void AWebsiteWithoutSectionsStillRendersAValidEmptyList()
    {
        string nav = NavigationOf();

        Assert.Equal("<nav><ul></ul></nav>", nav);
    }
}
