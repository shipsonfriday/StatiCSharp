using System;
using StatiCSharp.Tools;
using Xunit;

namespace StatiCSharp.Tests;

/// <summary>
/// The url of a site is also where it is written, so what this builds decides both.
/// </summary>
public class UrlPathTests
{
    [Theory]
    [InlineData("/posts/my-post", "posts", "my-post")]
    [InlineData("/my-section", "My Section")]
    [InlineData("/my-docs/sub-folder/a-page", "My Docs/Sub Folder", "a-page")]
    [InlineData("/docs/guide/setup", "docs", "guide/setup")]
    [InlineData("/", "")]
    public void FromJoinsSluggedSegments(string expected, params string[] parts)
    {
        Assert.Equal(expected, UrlPath.From(parts));
    }

    [Theory]
    [InlineData("../..")]
    [InlineData("../../escaped/..")]
    [InlineData("/etc/passwd")]
    [InlineData("..\\..\\escaped")]
    public void NothingLeavesTheSiteRoot(string part)
    {
        // A path entry in the front matter used to be used as given, so "../.." wrote the file
        // outside the output directory, where the clean up never looks.
        string url = UrlPath.From("posts", part);

        Assert.DoesNotContain("..", url, StringComparison.Ordinal);
        Assert.StartsWith("/posts", url, StringComparison.Ordinal);
    }

    [Fact]
    public void AWindowsHierarchyBecomesForwardSlashes()
    {
        Assert.Equal("/docs/guide/a-page", UrlPath.From(@"docs\guide", "a-page"));
    }

    [Fact]
    public void SegmentsOfIsWhatFromPutIn()
    {
        Assert.Equal(["posts", "my-post"], UrlPath.SegmentsOf(UrlPath.From("Posts", "My Post")));
        Assert.Empty(UrlPath.SegmentsOf("/"));
    }

    [Theory]
    [InlineData("posts", true)]
    [InlineData("My Section", true)]
    [InlineData("a/b", true)]
    [InlineData("", false)]
    [InlineData("+++", false)]
    [InlineData("...", false)]
    [InlineData("../..", false)]
    public void HasASegmentSaysWhetherAnythingUsableIsLeft(string part, bool expected)
    {
        Assert.Equal(expected, UrlPath.HasASegment(part));
    }

    [Fact]
    public void RejectsNull()
    {
        Assert.Throws<ArgumentNullException>(() => UrlPath.From(null!));
        Assert.Throws<ArgumentNullException>(() => UrlPath.SegmentsOf(null!));
        Assert.Throws<ArgumentNullException>(() => UrlPath.HasASegment(null!));
    }

    [Theory]
    [InlineData("a-custom-path", "a-custom-path", "a-post.md")]
    [InlineData("a-post", "", "A Post.md")]
    [InlineData("a-post", "...", "A Post.md")]
    [InlineData("a-post", "../..", "A Post.md")]
    public void TheSegmentFallsBackToTheFilename(string expected, string path, string filename)
    {
        // A path entry that keeps nothing usable would leave the site without a segment of its
        // own, so it would be written over the page in the directory above it.
        Assert.Equal(expected, SiteSegment.Of(path, filename));
    }
}
