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
        Assert.Equal([".git", ".nojekyll", "CNAME"], manager.PreservedOutput);
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
            .WithPreservedOutput("robots.txt")
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
    public void WithPreservedOutput_AddsInsteadOfReplacing()
    {
        // The built in names are not a default a caller can overwrite by accident: losing
        // .git to a careless call is worse than having no way to drop it.
        WebsiteManager manager = WebsiteManager.For(AWebsite(), "source")
            .WithPreservedOutput("robots.txt")
            .WithPreservedOutput("  .well-known  ", "", "   ");

        Assert.Equal([".git", ".nojekyll", "CNAME", "robots.txt", ".well-known"], manager.PreservedOutput);
    }

    [Fact]
    public void WithPreservedOutput_RejectsNull()
    {
        Assert.Throws<ArgumentNullException>(
            () => WebsiteManager.For(AWebsite(), "source").WithPreservedOutput(null!));
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
        public string MakeHeadHtml(ISite site, RenderContext context) => string.Empty;
        public string MakeIndexHtml(IIndex index, RenderContext context) => string.Empty;
        public string MakePageHtml(IPage page, RenderContext context) => string.Empty;
        public string MakeSectionHtml(ISection section, RenderContext context) => string.Empty;
        public string MakeItemHtml(IItem item, RenderContext context) => string.Empty;
        public string MakeTagListHtml(IReadOnlyList<IItem> items, string tag, RenderContext context) => string.Empty;
    }
}
