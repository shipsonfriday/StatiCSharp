using System.Globalization;
using StatiCSharp.Interfaces;
using Xunit;

namespace StatiCSharp.Tests;

public class DefaultHtmlFactoryDateTests
{
    private static Item AMarchItem() => new()
    {
        Title = "A Post",
        Section = "posts",
        Date = new DateOnly(2020, 3, 4),
    };

    [Theory]
    // Wording and arrangement, see DateTextTests. The theme used to write "MMMM dd, yyyy",
    // which took the month name from the culture but kept the English order.
    [InlineData("en-US", "March 4, 2020")]
    [InlineData("de-DE", "4. März 2020")]
    [InlineData("fr-FR", "4 mars 2020")]
    public void TheRenderedDateFollowsTheWebsiteLanguage(string language, string expected)
    {
        Website website = Website.Create(url: "https://example.com", name: "My Website")
            .WithLanguage(language);

        string html = new DefaultHtmlFactory().MakeItemHtml(AMarchItem(), new RenderContext { Website = website });

        Assert.Contains(expected, html, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("de-DE")]
    [InlineData("fr-FR")]
    public void TheRenderedDateDoesNotFollowTheBuildMachine(string machineCulture)
    {
        // The month name used to come from the culture of whichever machine ran the
        // generator, so an English site built on a German machine read "März".
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(machineCulture);

            Website website = Website.Create(url: "https://example.com", name: "My Website")
                .WithLanguage("en-US");

            string html = new DefaultHtmlFactory().MakeItemHtml(AMarchItem(), new RenderContext { Website = website });

            Assert.Contains("March 4, 2020", html, StringComparison.Ordinal);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void TheItemListUsesTheWebsiteLanguageToo()
    {
        Website website = Website.Create(url: "https://example.com", name: "My Website")
            .WithLanguage("de-DE");

        List<IItem> items = [AMarchItem()];
        string html = new DefaultHtmlFactory().MakeTagListHtml(items, "a-tag", new RenderContext { Website = website });

        Assert.Contains("4. März 2020", html, StringComparison.Ordinal);
    }
}
