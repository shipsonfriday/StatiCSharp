using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace StatiCSharp.Tests;

/// <summary>
/// First test over an internal type, which also proves that InternalsVisibleTo is in place.
/// </summary>
public class MarkdownFactoryTests
{
    [Fact]
    public void ParseMetaData_ReadsKeysFromFrontMatter()
    {
        string path = WriteTempMarkdown(
            "---",
            "title: My Post",
            "author: Roland",
            "---",
            "# Heading");

        try
        {
            Dictionary<string, string> metaData = MarkdownFactory.ParseMetaData(path);

            Assert.Equal("My Post", metaData["title"]);
            Assert.Equal("Roland", metaData["author"]);
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(path)!, recursive: true);
        }
    }

    /// <summary>
    /// Writes the given lines to a markdown file in a throwaway directory.
    /// MarkdownFactory only takes paths, so the test needs a real file on disk.
    /// </summary>
    private static string WriteTempMarkdown(params string[] lines)
    {
        string directory = Path.Combine(Path.GetTempPath(), $"staticsharp-tests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);

        string path = Path.Combine(directory, "content.md");
        File.WriteAllText(path, string.Join("\n", lines));

        return path;
    }
}
