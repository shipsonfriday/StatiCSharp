using StatiCSharp.HtmlComponents;
using Xunit;

namespace StatiCSharp.Tests;

public class HtmlElementEscapingTests
{
    [Fact]
    public void Render_EncodesAQuoteInAnAttributeValue()
    {
        // Unencoded, the quote ends the attribute early and the rest of the value
        // leaks out as markup.
        string html = new Div("body").Class("a\"b").Render();

        Assert.Equal("<div class=\"a&quot;b\">body</div>", html);
    }

    [Fact]
    public void Render_EncodesAnAmpersandInAnAttributeValue()
    {
        // A tag name from the front matter reaches href unchecked.
        string html = new A("link").Href("/tag/a&b").Render();

        Assert.Equal("<a href=\"/tag/a&amp;b\">link</a>", html);
    }

    [Fact]
    public void Render_EncodesAngleBracketsInAnAttributeValue()
    {
        string html = new Div("body").Id("x<y>").Render();

        Assert.Equal("<div id=\"x&lt;y&gt;\">body</div>", html);
    }

    [Fact]
    public void Render_LeavesAnOrdinaryValueAlone()
    {
        string html = new Div("body").Class("wrapper").Render();

        Assert.Equal("<div class=\"wrapper\">body</div>", html);
    }

    [Fact]
    public void Render_StillWritesValuelessAttributesAsBareKeys()
    {
        string html = new Div("body").Hidden().Render();

        Assert.Equal("<div hidden>body</div>", html);
    }

    [Fact]
    public void Render_StillOmitsTheClosingTagOfAVoidElement()
    {
        string html = new Input().Type("text").Render();

        Assert.Equal("<input type=\"text\">", html);
    }

    [Fact]
    public void Render_DoesNotTouchTheContent()
    {
        // Text renders verbatim on purpose: the content of a site is already rendered
        // html by the time it gets here.
        string html = new Div("<p>already html</p>").Render();

        Assert.Equal("<div><p>already html</p></div>", html);
    }
}
