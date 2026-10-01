using System;
using System.Collections.Generic;
using System.IO;
using StatiCSharp.Interfaces;
using Xunit;

namespace StatiCSharp.Tests;

public class WebsiteManagerTests
{
    private static Website AWebsite() =>
        Website.Create(url: "https://example.com", name: "My Website");

    [Fact]
    public void For_DerivesTheThreeDirectoriesFromTheSource()
    {
        string source = Path.Combine("home", "roland", "project");

        WebsiteManager manager = WebsiteManager.For(AWebsite(), source);

        Assert.Equal(source, manager.SourceDir);
        Assert.Equal(Path.Combine(source, "Content"), manager.Content);
        Assert.Equal(Path.Combine(source, "Resources"), manager.Resources);
        Assert.Equal(Path.Combine(source, "Output"), manager.Output);
    }

    [Fact]
    public void For_AppliesDefaultsForEverythingOptional()
    {
        WebsiteManager manager = WebsiteManager.For(AWebsite(), "source");

        Assert.True(manager.IncrementalOutput);
        Assert.True(manager.UseDefaultMarkdownParser);
        Assert.IsType<DefaultHtmlFactory>(manager.HtmlFactory);
    }

    [Fact]
    public void For_RejectsAMissingWebsite()
    {
        Assert.Throws<ArgumentNullException>(() => WebsiteManager.For(null!, "source"));
    }

    [Fact]
    public void For_RejectsANullSource()
    {
        Assert.Throws<ArgumentNullException>(() => WebsiteManager.For(AWebsite(), null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void For_RejectsABlankSource(string source)
    {
        Assert.Throws<ArgumentException>(() => WebsiteManager.For(AWebsite(), source));
    }

    [Fact]
    public void WithMethods_ChainAndReturnTheSameInstance()
    {
        WebsiteManager manager = WebsiteManager.For(AWebsite(), "source");
        var theme = new StubHtmlFactory();

        WebsiteManager result = manager
            .WithTheme(theme)
            .NoIncrementalOutput()
            .WithContentDirectory("other/Content")
            .WithResourcesDirectory("other/Resources")
            .WithOutputDirectory("other/Output")
            .NoDefaultMarkdownParser();

        Assert.Same(manager, result);
        Assert.Same(theme, manager.HtmlFactory);
        Assert.False(manager.IncrementalOutput);
        Assert.Equal("other/Content", manager.Content);
        Assert.Equal("other/Resources", manager.Resources);
        Assert.Equal("other/Output", manager.Output);
        Assert.False(manager.UseDefaultMarkdownParser);
    }

    [Fact]
    public void NoIncrementalOutput_SaysSoMoreThanOnce()
    {
        // The methods that switch something off are not toggles. Calling one twice says the
        // same thing twice, it does not say the opposite the second time.
        WebsiteManager manager = WebsiteManager.For(AWebsite(), "source")
            .NoIncrementalOutput()
            .NoIncrementalOutput();

        Assert.False(manager.IncrementalOutput);
    }

    [Fact]
    public void DirectoryOverrides_DoNotChangeTheSourceDirectory()
    {
        WebsiteManager manager = WebsiteManager.For(AWebsite(), "source")
            .WithOutputDirectory("somewhere/else");

        Assert.Equal("source", manager.SourceDir);
        Assert.Equal(Path.Combine("source", "Content"), manager.Content);
    }

    private sealed class StubHtmlFactory : IHtmlFactory
    {
        public string ResourcesPath => "resources";
        public string MakeHeadHtml() => string.Empty;
        public string MakeIndexHtml(IIndex index) => string.Empty;
        public string MakePageHtml(IPage page) => string.Empty;
        public string MakeSectionHtml(ISection section) => string.Empty;
        public string MakeItemHtml(IItem item) => string.Empty;
        public string MakeTagListHtml(List<IItem> items, string tag) => string.Empty;
    }
}
