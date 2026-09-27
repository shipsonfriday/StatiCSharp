using System;
using System.Collections.Generic;
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
    [InlineData("en-US", "March 04, 2020")]
    [InlineData("de-DE", "März 04, 2020")]
    [InlineData("fr-FR", "mars 04, 2020")]
    public void TheRenderedDateFollowsTheWebsiteLanguage(string language, string expected)
    {
        Website website = Website.Create(url: "https://example.com", name: "My Website")
            .WithLanguage(language);

        string html = new DefaultHtmlFactory(website).MakeItemHtml(AMarchItem());

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

            string html = new DefaultHtmlFactory(website).MakeItemHtml(AMarchItem());

            Assert.Contains("March 04, 2020", html, StringComparison.Ordinal);
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
        string html = new DefaultHtmlFactory(website).MakeTagListHtml(items, "a-tag");

        Assert.Contains("März 04, 2020", html, StringComparison.Ordinal);
    }
}
