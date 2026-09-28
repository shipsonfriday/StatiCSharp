using StatiCSharp.Tools;
using Xunit;

namespace StatiCSharp.Tests;

public class ItemTests
{
    [Fact]
    public void Url_DerivesTheSegmentFromTheMarkdownFilename()
    {
        var item = new Item { Section = "posts", MarkdownFileName = "My Post.md" };

        Assert.Equal("/posts/my-post", item.Url);
    }

    [Fact]
    public void Url_IsLowercasedLikeTheDirectoryThatGetsWritten()
    {
        // The url used to keep the original casing while the output directory was
        // lowercased, so every link with an uppercase letter in the filename ended up
        // pointing nowhere on a case-sensitive server.
        var item = new Item { Section = "posts", MarkdownFileName = "My-First-POST.md" };

        Assert.Equal("/posts/my-first-post", item.Url);
    }

    [Fact]
    public void Url_AgreesWithThePathTheWriterBuilds()
    {
        var item = new Item { Section = "posts", MarkdownFileName = "Another Post.md" };

        // WebsiteManager.MakeItems builds the output directory from exactly this call.
        string writtenSegment = FilenameToPath.From(item.MarkdownFileName);

        Assert.Equal($"/posts/{writtenSegment}", item.Url);
    }

    [Fact]
    public void Url_PrefersThePathFromMetaData()
    {
        var item = new Item
        {
            Section = "posts",
            MarkdownFileName = "My Post.md",
            Path = "a-custom-path",
        };

        Assert.Equal("/posts/a-custom-path", item.Url);
    }

    [Fact]
    public void Url_DoesNotThrowForAFilenameWithoutAnExtension()
    {
        var item = new Item { Section = "posts", MarkdownFileName = "no-extension" };

        Assert.Equal("/posts/no-extension", item.Url);
    }
}
