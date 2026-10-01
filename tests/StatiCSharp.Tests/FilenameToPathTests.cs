using System.Globalization;
using StatiCSharp.Tools;
using Xunit;

namespace StatiCSharp.Tests;

public class FilenameToPathTests
{
    [Theory]
    [InlineData("My Post.md", "my-post")]
    [InlineData("index.md", "index")]
    [InlineData("UPPERCASE.md", "uppercase")]
    [InlineData("archive.tar.gz", "archive.tar")]
    public void From_DropsTheExtensionAndLowercases(string filename, string expected)
    {
        Assert.Equal(expected, FilenameToPath.From(filename));
    }

    [Fact]
    public void From_DoesNotLeaveATrailingHyphen()
    {
        // The space before the extension used to survive as a hyphen, because the
        // trimming ran after spaces had already been replaced.
        Assert.Equal("my-post", FilenameToPath.From("My Post .md"));
    }

    [Theory]
    [InlineData("no-extension", "no-extension")]
    [InlineData("", "")]
    [InlineData(".md", "")]
    public void From_HandlesFilenamesWithoutAnExtension(string filename, string expected)
    {
        // Used to throw ArgumentOutOfRangeException, because LastIndexOf returned -1.
        Assert.Equal(expected, FilenameToPath.From(filename));
    }

    [Fact]
    public void From_RejectsNull()
    {
        Assert.Throws<ArgumentNullException>(() => FilenameToPath.From(null!));
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("tr-TR")]
    [InlineData("de-DE")]
    public void From_GivesTheSameResultInEveryCulture(string culture)
    {
        // Turkish is the interesting one: a culture-sensitive ToLower turns "I" into the
        // dotless "ı", so the same file would get a different url on a Turkish machine.
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);

            Assert.Equal("istanbul", FilenameToPath.From("ISTANBUL.md"));
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }
}
