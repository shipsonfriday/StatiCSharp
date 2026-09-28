using Xunit;

namespace StatiCSharp.Tests;

public class PageTests
{
    [Fact]
    public void Url_JoinsHierarchyAndSegment()
    {
        var page = new Page { Hierarchy = "about", MarkdownFileName = "Another Page.md" };

        Assert.Equal("/about/another-page", page.Url);
    }

    [Fact]
    public void Url_DropsAnIndexSegment()
    {
        // about/index.md is written to /about. The url used to say /about/index, which
        // is a directory that never gets created.
        var page = new Page { Hierarchy = "about", MarkdownFileName = "index.md" };

        Assert.Equal("/about", page.Url);
    }

    [Fact]
    public void Url_UsesForwardSlashesForANestedHierarchy()
    {
        // Hierarchy is a file system path. On Windows it arrives with backslashes, which
        // used to end up verbatim in the url.
        var page = new Page { Hierarchy = @"docs\guide", MarkdownFileName = "setup.md" };

        Assert.Equal("/docs/guide/setup", page.Url);
    }

    [Fact]
    public void Url_DropsAnIndexSegmentInANestedHierarchyToo()
    {
        var page = new Page { Hierarchy = @"docs\guide", MarkdownFileName = "index.md" };

        Assert.Equal("/docs/guide", page.Url);
    }

    [Fact]
    public void Url_PrefersThePathFromMetaData()
    {
        var page = new Page
        {
            Hierarchy = "about",
            MarkdownFileName = "Another Page.md",
            Path = "a-custom-path",
        };

        Assert.Equal("/about/a-custom-path", page.Url);
    }

    [Fact]
    public void Url_DropsAnIndexPathFromMetaDataAsWell()
    {
        // MakePages applies the index rule after resolving the meta data path, so this
        // page is written to /about too.
        var page = new Page
        {
            Hierarchy = "about",
            MarkdownFileName = "Another Page.md",
            Path = "index",
        };

        Assert.Equal("/about", page.Url);
    }

    [Fact]
    public void Url_HandlesAnEmptyHierarchy()
    {
        var page = new Page { Hierarchy = string.Empty, MarkdownFileName = "standalone.md" };

        Assert.Equal("/standalone", page.Url);
    }
}
