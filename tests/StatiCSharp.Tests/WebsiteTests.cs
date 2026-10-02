using System;
using System.Globalization;
using Xunit;

namespace StatiCSharp.Tests;

public class WebsiteTests
{
    private static Website AWebsite() =>
        Website.Create(url: "https://example.com", name: "My Website");

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

    [Theory]
    [InlineData("en-US")]
    [InlineData("de-DE")]
    [InlineData("de")]
    [InlineData("sr-Cyrl-RS")]
    [InlineData("  en-GB  ")]
    public void WithLanguage_AcceptsALanguageThisMachineKnows(string language)
    {
        Website website = AWebsite().WithLanguage(language);

        Assert.Equal(language.Trim(), website.Language.Name);
    }

    [Theory]
    [InlineData("klingon")]
    [InlineData("xx-YY")]
    [InlineData("not a language at all")]
    public void WithLanguage_RejectsATagNoMachineKnows(string language)
    {
        // new CultureInfo(tag) invents a culture for anything that looks like a tag: it
        // succeeded, wrote lang="klingon" into every page and formatted dates in English.
        Assert.Throws<CultureNotFoundException>(() => AWebsite().WithLanguage(language));
    }

    [Theory]
    [InlineData("en_US")]
    [InlineData("de_DE")]
    public void WithLanguage_RejectsAnUnderscoreAndSaysWhatWasMeant(string language)
    {
        // .NET reads this as a sort order and accepts it, so lang="en_us" reached the page -
        // not a valid language tag.
        var thrown = Assert.Throws<ArgumentException>(() => AWebsite().WithLanguage(language));

        Assert.Contains(language.Replace('_', '-'), thrown.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void WithLanguage_RejectsTheInvariantCulture()
    {
        // It would leave lang="" on every page.
        Assert.Throws<ArgumentException>(() => AWebsite().WithLanguage(CultureInfo.InvariantCulture));
    }

    [Fact]
    public void WithLanguage_ChecksACultureItIsHandedDirectly()
    {
        // The overload taking a CultureInfo is the same gate: a made-up culture must not slip
        // past it just because the caller built it themselves.
        Assert.Throws<CultureNotFoundException>(
            () => AWebsite().WithLanguage(new CultureInfo("xx-YY")));
    }

    [Fact]
    public void WithLanguage_RejectsNull()
    {
        Assert.Throws<ArgumentNullException>(() => AWebsite().WithLanguage((string)null!));
        Assert.Throws<ArgumentNullException>(() => AWebsite().WithLanguage((CultureInfo)null!));
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
