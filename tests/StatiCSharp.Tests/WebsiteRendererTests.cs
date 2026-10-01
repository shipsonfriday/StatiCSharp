using System;
using System.IO;
using System.Threading.Tasks;
using StatiCSharp.Interfaces;
using Xunit;

namespace StatiCSharp.Tests;

/// <summary>
/// Covers the writing half on its own. It used to be five partial files of the manager, so
/// reaching these paths meant running a whole generation from markdown files on disk.
/// </summary>
public class WebsiteRendererTests
{
    /// <summary>
    /// A context with one section holding one item per set of tags. No markdown files needed:
    /// the writing half takes the content as it is handed to it.
    /// </summary>
    private static RenderContext AContextWithTags(params string[][] tagsPerItem)
    {
        Section section = new() { SectionName = "posts", Title = "Posts" };

        for (int i = 0; i < tagsPerItem.Length; i++)
        {
            section.AddItem(new Item
            {
                Title = $"Post {i}",
                MarkdownFileName = $"post-{i}.md",
                Tags = [.. tagsPerItem[i]],
            });
        }

        return new RenderContext
        {
            Website = Website.Create(url: "https://example.com", name: "My Website").WithSections("posts"),
            Sections = [section],
        };
    }

    private static WebsiteRenderer RendererFor(RenderContext context, OutputWriter output, string outputDirectory)
        => new(context, new DefaultHtmlFactory(), new HtmlBuilder(useDefaultMarkdownParser: true), output, outputDirectory);

    [Fact]
    public void RejectsMissingArguments()
    {
        RenderContext context = AContextWithTags();
        var factory = new DefaultHtmlFactory();
        var builder = new HtmlBuilder(useDefaultMarkdownParser: true);
        var output = new OutputWriter("/output", onlyWriteWhatChanged: true);

        Assert.Throws<ArgumentNullException>(() => new WebsiteRenderer(null!, factory, builder, output, "/output"));
        Assert.Throws<ArgumentNullException>(() => new WebsiteRenderer(context, null!, builder, output, "/output"));
        Assert.Throws<ArgumentNullException>(() => new WebsiteRenderer(context, factory, null!, output, "/output"));
        Assert.Throws<ArgumentNullException>(() => new WebsiteRenderer(context, factory, builder, null!, "/output"));
        Assert.Throws<ArgumentNullException>(() => new WebsiteRenderer(context, factory, builder, output, null!));
        Assert.Throws<ArgumentException>(() => new WebsiteRenderer(context, factory, builder, output, "   "));
    }

    [Fact]
    public async Task ATagWithNoUsableCharactersIsSkipped()
    {
        // An empty slug would make the path /tag, so the tag page would overwrite the list
        // of tags itself.
        using var directory = new TempDirectory();
        RenderContext context = AContextWithTags(["+++"]);

        var output = new OutputWriter(directory.Path, onlyWriteWhatChanged: true);
        await RendererFor(context, output, directory.Path).RenderTagListsAsync();

        Assert.False(File.Exists(Path.Combine(directory.Path, "tag", "index.html")));
        Assert.False(Directory.Exists(Path.Combine(directory.Path, "tag")));
    }

    [Fact]
    public async Task EveryTagGetsItsOwnDirectory()
    {
        using var directory = new TempDirectory();
        RenderContext context = AContextWithTags(["CSharp", "Web Dev"]);

        var output = new OutputWriter(directory.Path, onlyWriteWhatChanged: true);
        await RendererFor(context, output, directory.Path).RenderTagListsAsync();

        Assert.True(File.Exists(Path.Combine(directory.Path, "tag", "csharp", "index.html")));
        Assert.True(File.Exists(Path.Combine(directory.Path, "tag", "web-dev", "index.html")));
    }

    [Fact]
    public async Task TwoTagsSharingASlugEndUpInOneDirectory()
    {
        // Reported as a warning rather than silently losing one of them. Which one wins is
        // not defined, so the test only pins down that there is exactly one.
        using var directory = new TempDirectory();
        RenderContext context = AContextWithTags(["Web Dev"], ["web-dev"]);

        var output = new OutputWriter(directory.Path, onlyWriteWhatChanged: true);
        await RendererFor(context, output, directory.Path).RenderTagListsAsync();

        Assert.Single(Directory.GetDirectories(Path.Combine(directory.Path, "tag")));
        Assert.True(File.Exists(Path.Combine(directory.Path, "tag", "web-dev", "index.html")));
    }

    [Fact]
    public async Task AnItemGoesBelowItsSection()
    {
        using var directory = new TempDirectory();
        RenderContext context = AContextWithTags(["one"]);

        var output = new OutputWriter(directory.Path, onlyWriteWhatChanged: true);
        await RendererFor(context, output, directory.Path).RenderItemsAsync();

        Assert.True(File.Exists(Path.Combine(directory.Path, "posts", "post-0", "index.html")));
    }

    [Fact]
    public async Task AnItemWithAPathInItsMetaDataGoesThere()
    {
        using var directory = new TempDirectory();
        RenderContext context = AContextWithTags(["one"]);
        ((Item)context.Sections[0].Items[0]).Path = "a-custom-path";

        var output = new OutputWriter(directory.Path, onlyWriteWhatChanged: true);
        await RendererFor(context, output, directory.Path).RenderItemsAsync();

        Assert.True(File.Exists(Path.Combine(directory.Path, "posts", "a-custom-path", "index.html")));
        Assert.False(Directory.Exists(Path.Combine(directory.Path, "posts", "post-0")));
    }

    [Fact]
    public async Task TwoItemsClaimingOneUrlLeaveOneFileBehind()
    {
        using var directory = new TempDirectory();
        RenderContext context = AContextWithTags(["one"], ["two"]);
        foreach (IItem item in context.Sections[0].Items)
        {
            ((Item)item).Path = "same-path";
        }

        var output = new OutputWriter(directory.Path, onlyWriteWhatChanged: true);
        await RendererFor(context, output, directory.Path).RenderItemsAsync();

        Assert.Single(Directory.GetDirectories(Path.Combine(directory.Path, "posts")));
    }
}
