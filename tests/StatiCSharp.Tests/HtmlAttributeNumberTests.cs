using System.Globalization;
using StatiCSharp.HtmlComponents;
using Xunit;

namespace StatiCSharp.Tests;

public class HtmlAttributeNumberTests
{
    private static string InCulture(string culture, Func<string> render)
    {
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);
            return render();
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("sv-SE")]
    [InlineData("ar-SA")]
    [InlineData("fa-IR")]
    public void ANegativeTabIndexKeepsAnAsciiHyphen(string culture)
    {
        // sv-SE renders a negative int with U+2212 MINUS SIGN and the Arabic cultures add
        // a direction mark, so tabindex="-1" - a common html idiom - became unparsable.
        string html = InCulture(culture, () => new Div("body").TabIndex(-1).Render());

        Assert.Contains("tabindex=\"-1\"", html, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("de-DE")]
    [InlineData("fr-FR")]
    [InlineData("ar-SA")]
    public void AFractionalMinimumKeepsADecimalPoint(string culture)
    {
        // de-DE and fr-FR write 1,5 and the Arabic cultures write 1٫5, neither of which an
        // html number input accepts.
        string html = InCulture(culture, () => new Input().Type("number").Min(1.5f).Render());

        Assert.Contains("min=\"1.5\"", html, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("de-DE")]
    public void AFractionalMaximumKeepsADecimalPoint(string culture)
    {
        string html = InCulture(culture, () => new Input().Type("number").Max(2.5f).Render());

        Assert.Contains("max=\"2.5\"", html, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("de-DE")]
    [InlineData("sv-SE")]
    public void SizeWidthAndHeightAreFormattedTheSameEverywhere(string culture)
    {
        string html = InCulture(culture, () => new Div("body").Width(1024).Height(768).Render());

        Assert.Contains("width=\"1024\"", html, StringComparison.Ordinal);
        Assert.Contains("height=\"768\"", html, StringComparison.Ordinal);
    }
}
