using System.Globalization;
using StatiCSharp.Tools;
using Xunit;

namespace StatiCSharp.Tests;

public class UrlSlugTests
{
    [Theory]
    [InlineData("Web Dev", "web-dev")]
    [InlineData("csharp", "csharp")]
    [InlineData("CSharp", "csharp")]
    [InlineData("  padded  ", "padded")]
    [InlineData("node.js", "node.js")]
    [InlineData("under_score", "under_score")]
    [InlineData("already-hyphenated", "already-hyphenated")]
    public void From_KeepsWhatIsUsableAndLowercases(string text, string expected)
    {
        Assert.Equal(expected, UrlSlug.From(text));
    }

    [Theory]
    [InlineData("c/c++", "c-c")]
    [InlineData("a?b#c", "a-b-c")]
    [InlineData("back\\slash", "back-slash")]
    [InlineData("colon:name", "colon-name")]
    [InlineData("100% sure", "100-sure")]
    public void From_ReplacesPathAndUrlSeparatorsWithASingleHyphen(string text, string expected)
    {
        // A slash used to nest the tag directory, and #, ? or % end or change the url.
        Assert.Equal(expected, UrlSlug.From(text));
    }

    [Theory]
    [InlineData("many    spaces", "many-spaces")]
    [InlineData("a -- b", "a-b")]
    [InlineData("-leading and trailing-", "leading-and-trailing")]
    [InlineData("trailing dot.", "trailing-dot")]
    public void From_CollapsesAndTrimsSeparators(string text, string expected)
    {
        // A trailing dot is not a valid directory name on Windows.
        Assert.Equal(expected, UrlSlug.From(text));
    }

    [Theory]
    [InlineData("Программирование", "программирование")]
    [InlineData("Ελληνικά", "ελληνικά")]
    [InlineData("Café Küche", "café-küche")]
    public void From_KeepsLettersOfAnyScript(string text, string expected)
    {
        // Stripping non-ASCII would collapse a whole tag to nothing.
        Assert.Equal(expected, UrlSlug.From(text));
    }

    [Theory]
    [InlineData("+++", "")]
    [InlineData("   ", "")]
    [InlineData("", "")]
    public void From_CanEndUpEmpty(string text, string expected)
    {
        Assert.Equal(expected, UrlSlug.From(text));
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("tr-TR")]
    public void From_GivesTheSameResultInEveryCulture(string culture)
    {
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);

            Assert.Equal("istanbul", UrlSlug.From("ISTANBUL"));
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void From_RejectsNull()
    {
        Assert.Throws<ArgumentNullException>(() => UrlSlug.From(null!));
    }
}
