using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace StatiCSharp.Tests;

public class MarkdownFactoryTests
{
    [Fact]
    public void ParseMetaData_ReadsKeysFromFrontMatter()
    {
        using var directory = new TempDirectory();
        string path = directory.WriteFile(
            "content.md",
            "---",
            "title: My Post",
            "author: Roland",
            "---",
            "# Heading");

        Dictionary<string, string> metaData = MarkdownFactory.ParseMetaData(path);

        Assert.Equal("My Post", metaData["title"]);
        Assert.Equal("Roland", metaData["author"]);
    }

    [Fact]
    public void ParseContent_SkipsTheFrontMatter()
    {
        using var directory = new TempDirectory();
        string path = directory.WriteFile("content.md", "---", "title: My Post", "---", "# Heading", "Body.");

        Assert.Equal("# Heading\nBody.", MarkdownFactory.ParseContent(path));
    }

    [Fact]
    public void AnEmptyFileIsNotAnError()
    {
        // lines[0] used to throw IndexOutOfRangeException, so an empty placeholder file
        // took the whole generator down.
        using var directory = new TempDirectory();
        string path = directory.WriteFile("content.md");

        Assert.Empty(MarkdownFactory.ParseMetaData(path));
        Assert.Equal(string.Empty, MarkdownFactory.ParseContent(path));
    }

    [Fact]
    public void ADuplicateKeyKeepsTheLastValueAndTheRestOfTheMetaData()
    {
        // metaData.Add threw on the duplicate, and the try wrapped the whole loop, so
        // every line after it was lost - here that was the author.
        using var directory = new TempDirectory();
        string path = directory.WriteFile(
            "content.md",
            "---",
            "title: First",
            "title: Second",
            "author: Roland",
            "---",
            "Body.");

        Dictionary<string, string> metaData = MarkdownFactory.ParseMetaData(path);

        Assert.Equal("Second", metaData["title"]);
        Assert.Equal("Roland", metaData["author"]);
    }

    [Fact]
    public void ABlankLineInsideTheFrontMatterDoesNotDropTheRest()
    {
        // "".IndexOf(':') is -1, so Substring(0, -1) threw and aborted the loop.
        using var directory = new TempDirectory();
        string path = directory.WriteFile(
            "content.md",
            "---",
            "title: My Post",
            "",
            "author: Roland",
            "---",
            "Body.");

        Dictionary<string, string> metaData = MarkdownFactory.ParseMetaData(path);

        Assert.Equal("My Post", metaData["title"]);
        Assert.Equal("Roland", metaData["author"]);
    }

    [Fact]
    public void ALineWithoutAColonIsSkippedButTheRestIsRead()
    {
        using var directory = new TempDirectory();
        string path = directory.WriteFile(
            "content.md",
            "---",
            "title: My Post",
            "this line is malformed",
            "author: Roland",
            "---",
            "Body.");

        Dictionary<string, string> metaData = MarkdownFactory.ParseMetaData(path);

        Assert.Equal(2, metaData.Count);
        Assert.Equal("Roland", metaData["author"]);
    }

    [Fact]
    public void AYamlCommentIsIgnoredSilently()
    {
        using var directory = new TempDirectory();
        string path = directory.WriteFile(
            "content.md",
            "---",
            "# a comment",
            "title: My Post",
            "---",
            "Body.");

        Dictionary<string, string> metaData = MarkdownFactory.ParseMetaData(path);

        Assert.Equal("My Post", Assert.Single(metaData).Value);
    }

    [Fact]
    public void KeysAreLowercased()
    {
        using var directory = new TempDirectory();
        string path = directory.WriteFile("content.md", "---", "TITLE: My Post", "---", "Body.");

        Assert.Equal("My Post", MarkdownFactory.ParseMetaData(path)["title"]);
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("tr-TR")]
    public void KeysAreLowercasedTheSameWayInEveryCulture(string culture)
    {
        // A culture-sensitive ToLower turns "TITLE" into "tıtle" on a Turkish machine, and
        // the generator would never find the key again.
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);

            using var directory = new TempDirectory();
            string path = directory.WriteFile("content.md", "---", "TITLE: My Post", "---", "Body.");

            Assert.True(MarkdownFactory.ParseMetaData(path).ContainsKey("title"));
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void AValueMayContainAColon()
    {
        using var directory = new TempDirectory();
        string path = directory.WriteFile("content.md", "---", "title: Note: on urls", "---", "Body.");

        Assert.Equal("Note: on urls", MarkdownFactory.ParseMetaData(path)["title"]);
    }

    [Fact]
    public void ASingleMarkerCountsAsNoFrontMatter()
    {
        using var directory = new TempDirectory();
        string path = directory.WriteFile("content.md", "---", "title: My Post", "Body.");

        Assert.Empty(MarkdownFactory.ParseMetaData(path));
        Assert.Equal("---\ntitle: My Post\nBody.", MarkdownFactory.ParseContent(path));
    }

    [Fact]
    public void AFileWithoutFrontMatterIsAllContent()
    {
        using var directory = new TempDirectory();
        string path = directory.WriteFile("content.md", "# Heading", "Body.");

        Assert.Empty(MarkdownFactory.ParseMetaData(path));
        Assert.Equal("# Heading\nBody.", MarkdownFactory.ParseContent(path));
    }

    [Fact]
    public void RejectsNull()
    {
        Assert.Throws<ArgumentNullException>(() => MarkdownFactory.ParseMetaData(null!));
        Assert.Throws<ArgumentNullException>(() => MarkdownFactory.ParseContent(null!));
    }
}
