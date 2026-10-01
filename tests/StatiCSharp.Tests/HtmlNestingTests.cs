using StatiCSharp.HtmlComponents;
using StatiCSharp.Interfaces;
using Xunit;

namespace StatiCSharp.Tests;

/// <summary>
/// Covers the nesting syntax: children passed as constructor arguments.
/// </summary>
public class HtmlNestingTests
{
    [Fact]
    public void ChildrenCanBePassedAsConstructorArguments()
    {
        string html = new Div(
            new H1("Title"),
            new Paragraph("Text")).Render();

        Assert.Equal("<div><h1>Title</h1><p>Text</p></div>", html);
    }

    [Fact]
    public void NestingGoesAsDeepAsNeededAndAttributesStillAttach()
    {
        string html = new Body(
            new Div(
                new Article(
                    new H1("Title"),
                    new Div(new Text("<p>Body</p>")).Class("content"))).Class("wrapper")).Render();

        Assert.Equal(
            "<body><div class=\"wrapper\"><article><h1>Title</h1>"
            + "<div class=\"content\"><p>Body</p></div></article></div></body>",
            html);
    }

    [Fact]
    public void AnEmptyArgumentListRendersAnEmptyElement()
    {
        Assert.Equal("<div></div>", new Div().Render());
    }

    [Fact]
    public void ASingleComponentStillPicksTheDedicatedConstructor()
    {
        // new Div(component) has both an IHtmlComponent and a params overload available.
        // Overload resolution prefers the exact match, and both produce the same html.
        var child = new H1("Title");

        Assert.Equal(new Div(child).Render(), new Div([child]).Render());
    }

    [Fact]
    public void AStringArgumentStillPicksTheTextConstructor()
    {
        Assert.Equal("<div>plain</div>", new Div("plain").Render());
    }

    [Fact]
    public void TheInitializerFormProducesTheSameHtml()
    {
        string initializer = new Div { new H1("Title"), new Paragraph("Text") }.Render();
        string nested = new Div(new H1("Title"), new Paragraph("Text")).Render();

        Assert.Equal(nested, initializer);
    }

    [Fact]
    public void ListsNestTheSameWay()
    {
        string html = new Ul(
            new Li("first"),
            new Li(new A("second").Href("/second"))).Class("items").Render();

        Assert.Equal(
            "<ul class=\"items\"><li>first</li><li><a href=\"/second\">second</a></li></ul>",
            html);
    }

    [Fact]
    public void RejectsANullArgumentList()
    {
        // The cast is needed because a bare null was already ambiguous between the
        // IHtmlComponent and the string constructor, before the params one existed.
        Assert.Throws<ArgumentNullException>(() => new Div((IHtmlComponent[])null!));
    }
}
