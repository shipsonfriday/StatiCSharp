using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StatiCSharp.Interfaces;
using Xunit;

namespace StatiCSharp.Tests;

/// <summary>
/// Covers what a theme is handed. A theme used to be given the website in its constructor,
/// before anything was read, and only saw content because the collections were the very same
/// objects the reader wrote into.
/// </summary>
public class RenderContextTests
{
    [Fact]
    public void OnlyTheWebsiteIsRequired()
    {
        // So a theme's own tests can build one with just what they care about.
        var context = new RenderContext { Website = AWebsite() };

        Assert.NotNull(context.Index);
        Assert.Empty(context.Pages);
        Assert.Empty(context.Sections);
    }

    [Fact]
    public async Task TheThemeIsHandedTheWholeContent()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Content/index.md", "---", "title: Home", "---", "Welcome.");
        directory.WriteFile("Content/about/index.md", "---", "title: About", "---", "About.");
        directory.WriteFile("Content/posts/index.md", "---", "title: Posts", "---", "Section.");
        directory.WriteFile("Content/posts/a-post.md", "---", "title: A Post", "---", "Body.");

        Website website = Website.Create(url: "https://example.com", name: "My Website")
            .WithSections("posts");

        var theme = new RecordingHtmlFactory { ResourcesPath = directory.EmptyDirectory("Theme") };
        await WebsiteManager.For(website, directory.Path).WithTheme(theme).MakeAsync();

        RenderContext context = Assert.Single(new HashSet<RenderContext>(theme.Contexts));

        Assert.Same(website, context.Website);
        Assert.Equal("Home", context.Index.Title);
        Assert.Equal("About", Assert.Single(context.Pages).Title);
        Assert.Equal("A Post", Assert.Single(Assert.Single(context.Sections).Items).Title);
    }

    [Fact]
    public async Task TheHeadIsAskedForEverySiteWithThatSite()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Content/index.md", "---", "title: Home", "---", "Welcome.");
        directory.WriteFile("Content/posts/index.md", "---", "title: Posts", "---", "Section.");
        directory.WriteFile("Content/posts/a-post.md", "---", "title: A Post", "tags: one", "---", "Body.");

        Website website = Website.Create(url: "https://example.com", name: "My Website")
            .WithSections("posts");

        var theme = new RecordingHtmlFactory { ResourcesPath = directory.EmptyDirectory("Theme") };
        await WebsiteManager.For(website, directory.Path).WithTheme(theme).MakeAsync();

        // index, section, item, tag list - each with its own site, so a theme can put a
        // canonical url or open graph tags into the head. Sorted, because the items and the
        // tag lists are written through Task.WhenAll and their order among themselves is not
        // defined. Ordinal, because the default comparer sorts by culture - "one" comes
        // before "Posts" in some and after it in others.
        Assert.Equal(
            ["A Post", "Home", "Posts", "one | My Website"],
            [.. theme.HeadTitles.Order(StringComparer.Ordinal)]);
    }

    private static Website AWebsite() =>
        Website.Create(url: "https://example.com", name: "My Website");

    /// <summary>
    /// A theme that renders nothing and remembers what it was given.
    /// </summary>
    private sealed class RecordingHtmlFactory : IHtmlFactory
    {
        public List<RenderContext> Contexts { get; } = [];
        public List<string> HeadTitles { get; } = [];

        /// <summary>
        /// An empty directory. Everything below it is copied into the output, so this must
        /// never point at a directory that holds anything.
        /// </summary>
        public required string ResourcesPath { get; init; }

        public string MakeHeadHtml(ISite site, RenderContext context)
        {
            HeadTitles.Add(site.Title);
            return Record(context);
        }

        public string MakeIndexHtml(IIndex index, RenderContext context) => Record(context);
        public string MakePageHtml(IPage page, RenderContext context) => Record(context);
        public string MakeSectionHtml(ISection section, RenderContext context) => Record(context);
        public string MakeItemHtml(IItem item, RenderContext context) => Record(context);
        public string MakeTagListHtml(List<IItem> items, string tag, RenderContext context) => Record(context);

        private string Record(RenderContext context)
        {
            Contexts.Add(context);
            return string.Empty;
        }
    }
}
