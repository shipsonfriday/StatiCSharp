using System.Collections.Generic;
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
}
