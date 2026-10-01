using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Xunit;

namespace StatiCSharp.Tests;

public class MarkdownFactoryTests
{
    [Fact]
    public void Read_ReadsKeysFromFrontMatter()
    {
        using var directory = new TempDirectory();
        string path = directory.WriteFile(
            "content.md",
            "---",
            "title: My Post",
            "author: Roland",
            "---",
            "# Heading");

        Dictionary<string, string> metaData = MarkdownFactory.Read(path).MetaData;

        Assert.Equal("My Post", metaData["title"]);
        Assert.Equal("Roland", metaData["author"]);
    }

    [Fact]
    public void Read_SkipsTheFrontMatterInTheContent()
    {
        using var directory = new TempDirectory();
        string path = directory.WriteFile("content.md", "---", "title: My Post", "---", "# Heading", "Body.");

        Assert.Equal("# Heading\nBody.", MarkdownFactory.Read(path).Content);
    }

    [Fact]
    public void AnEmptyFileIsNotAnError()
    {
        // lines[0] used to throw IndexOutOfRangeException, so an empty placeholder file
        // took the whole generator down.
        using var directory = new TempDirectory();
        string path = directory.WriteFile("content.md");

        Assert.Empty(MarkdownFactory.Read(path).MetaData);
        Assert.Equal(string.Empty, MarkdownFactory.Read(path).Content);
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

        Dictionary<string, string> metaData = MarkdownFactory.Read(path).MetaData;

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

        Dictionary<string, string> metaData = MarkdownFactory.Read(path).MetaData;

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

        Dictionary<string, string> metaData = MarkdownFactory.Read(path).MetaData;

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

        Dictionary<string, string> metaData = MarkdownFactory.Read(path).MetaData;

        Assert.Equal("My Post", Assert.Single(metaData).Value);
    }

    [Fact]
    public void KeysAreLowercased()
    {
        using var directory = new TempDirectory();
        string path = directory.WriteFile("content.md", "---", "TITLE: My Post", "---", "Body.");

        Assert.Equal("My Post", MarkdownFactory.Read(path).MetaData["title"]);
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

            Assert.True(MarkdownFactory.Read(path).MetaData.ContainsKey("title"));
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

        Assert.Equal("Note: on urls", MarkdownFactory.Read(path).MetaData["title"]);
    }

    [Fact]
    public void ASingleMarkerCountsAsNoFrontMatter()
    {
        using var directory = new TempDirectory();
        string path = directory.WriteFile("content.md", "---", "title: My Post", "Body.");

        Assert.Empty(MarkdownFactory.Read(path).MetaData);
        Assert.Equal("---\ntitle: My Post\nBody.", MarkdownFactory.Read(path).Content);
    }

    [Fact]
    public void AFileWithoutFrontMatterIsAllContent()
    {
        using var directory = new TempDirectory();
        string path = directory.WriteFile("content.md", "# Heading", "Body.");

        Assert.Empty(MarkdownFactory.Read(path).MetaData);
        Assert.Equal("# Heading\nBody.", MarkdownFactory.Read(path).Content);
    }

    [Fact]
    public void AByteOrderMarkDoesNotHideTheFrontMatter()
    {
        // The markers are found by comparing the first line against exactly "---", so a file
        // saved with a BOM would carry it into that comparison. File.ReadAllLines consumes it;
        // this test is here so that a change of how the file is read cannot break that quietly.
        using var directory = new TempDirectory();
        string path = Path.Combine(directory.Path, "with-bom.md");
        File.WriteAllText(
            path,
            "---\ntitle: My Post\n---\nBody.",
            new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

        MarkdownFile file = MarkdownFactory.Read(path);

        Assert.Equal("My Post", file.MetaData["title"]);
        Assert.Equal("Body.", file.Content);
    }

    [Fact]
    public void RejectsNull()
    {
        Assert.Throws<ArgumentNullException>(() => MarkdownFactory.Read(null!));
    }
}
