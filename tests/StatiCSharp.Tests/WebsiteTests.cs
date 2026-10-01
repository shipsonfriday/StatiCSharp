using System.Globalization;
using Xunit;

namespace StatiCSharp.Tests;

public class WebsiteTests
{
    [Fact]
    public void Create_KeepsUrlAndName()
    {
        Website website = Website.Create(url: "https://example.com", name: "My Website");

        Assert.Equal("https://example.com", website.Url);
        Assert.Equal("My Website", website.Name);
    }

    [Fact]
    public void Create_AppliesDefaultsForEverythingOptional()
    {
        Website website = Website.Create(url: "https://example.com", name: "My Website");

        Assert.Equal(string.Empty, website.Description);
        Assert.Equal("en-US", website.Language.Name);
        Assert.Empty(website.MakeSectionsFor);
    }

    [Fact]
    public void Create_RejectsANullUrl()
    {
        Assert.Throws<ArgumentNullException>(() => Website.Create(url: null!, name: "My Website"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_RejectsABlankUrl(string url)
    {
        Assert.Throws<ArgumentException>(() => Website.Create(url: url, name: "My Website"));
    }

    [Fact]
    public void Create_RejectsANullName()
    {
        Assert.Throws<ArgumentNullException>(() => Website.Create(url: "https://example.com", name: null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_RejectsABlankName(string name)
    {
        Assert.Throws<ArgumentException>(() => Website.Create(url: "https://example.com", name: name));
    }

    [Fact]
    public void WithMethods_ChainAndReturnTheSameInstance()
    {
        Website website = Website.Create(url: "https://example.com", name: "My Website");

        Website result = website
            .WithDescription("A short description")
            .WithLanguage("de-DE")
            .WithSections("posts", "about");

        Assert.Same(website, result);
        Assert.Equal("A short description", website.Description);
        Assert.Equal("de-DE", website.Language.Name);
        Assert.Equal(["posts", "about"], website.MakeSectionsFor);
    }

    [Fact]
    public void WithLanguage_AlsoTakesACultureInfo()
    {
        Website website = Website.Create(url: "https://example.com", name: "My Website")
            .WithLanguage(new CultureInfo("fr-FR"));

        Assert.Equal("fr-FR", website.Language.Name);
    }

    [Fact]
    public void WithSections_TrimsEntriesAndDropsEmptyOnes()
    {
        Website website = Website.Create(url: "https://example.com", name: "My Website")
            .WithSections(" posts ", "", "   ", "about");

        Assert.Equal(["posts", "about"], website.MakeSectionsFor);
    }

    [Fact]
    public void WithSections_ReplacesWhatWasSetBefore()
    {
        Website website = Website.Create(url: "https://example.com", name: "My Website")
            .WithSections("posts")
            .WithSections("about");

        Assert.Equal(["about"], website.MakeSectionsFor);
    }
}
