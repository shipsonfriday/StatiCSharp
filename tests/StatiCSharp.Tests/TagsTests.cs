using System;
using StatiCSharp.HtmlComponents;
using StatiCSharp.Interfaces;
using Xunit;
using static StatiCSharp.HtmlComponents.Tags;

namespace StatiCSharp.Tests;

/// <summary>
/// Covers the static factories, which exist so a nested tree can be written without the
/// <c>new</c> keyword on every line.
/// </summary>
public class TagsTests
{
    [Fact]
    public void AFactoryProducesTheSameHtmlAsItsConstructor()
    {
        Assert.Equal(new Div("body").Render(), Div("body").Render());
    }

    [Fact]
    public void AWholeTreeIsIdenticalEitherWay()
    {
        string withNew = new Body(
                new Div(
                    new Article(
                        new H1("Title"),
                        new Div(new Text("<p>Body</p>")).Class("content"))).Class("wrapper"))
            .Render();

        string withoutNew = Body(
                Div(
                    Article(
                        H1("Title"),
                        Div(Text("<p>Body</p>")).Class("content"))).Class("wrapper"))
            .Render();

        Assert.Equal(withNew, withoutNew);
    }

    [Fact]
    public void TheFactoriesKeepTheDerivedTypeSoTheChainStillWorks()
    {
        Assert.Equal(
            "<a href=\"/posts\" class=\"nav\">Link</a>",
            A("Link").Href("/posts").Class("nav").Render());
    }

    [Fact]
    public void AnEmptyFactoryCallGivesAnEmptyElement()
    {
        Assert.Equal("<ul></ul>", Ul().Render());
    }

    [Fact]
    public void TheInitializerFormWorksOnAFactoryCallToo()
    {
        Assert.Equal("<div><h1>Title</h1></div>", new Div { H1("Title") }.Render());
    }

    [Fact]
    public void VoidElementsAndTextHaveFactoriesAsWell()
    {
        Assert.Equal("<input type=\"text\">", Input().Type("text").Render());
        Assert.Equal("<img src=\"/logo.png\">", Image("/logo.png").Render());
        Assert.Equal("plain", Text("plain").Render());
    }

    [Theory]
    [InlineData("A")]
    [InlineData("Article")]
    [InlineData("Body")]
    [InlineData("Div")]
    [InlineData("Footer")]
    [InlineData("Header")]
    [InlineData("H1")]
    [InlineData("Li")]
    [InlineData("Ul")]
    [InlineData("Ol")]
    [InlineData("Nav")]
    [InlineData("Span")]
    [InlineData("Input")]
    public void MostTypeNamesAlreadyMatchTheirTag(string typeName)
    {
        // Which is why only Paragraph and Image get a short form: there is nothing to
        // shorten anywhere else. If a new element breaks that, this test says so.
        Type type = typeof(Div).Assembly.GetType($"StatiCSharp.HtmlComponents.{typeName}")!;
        var element = (IHtmlComponent)Activator.CreateInstance(type)!;

        Assert.StartsWith($"<{typeName.ToLowerInvariant()}", element.Render(), StringComparison.Ordinal);
    }

    [Fact]
    public void PIsTheShortFormOfParagraph()
    {
        Assert.Equal(Paragraph("Text").Render(), P("Text").Render());
        Assert.Equal("<p>Text</p>", P("Text").Render());
        Assert.Equal("<p></p>", P().Render());
        Assert.Equal("<p><span>x</span></p>", P(Span("x")).Render());
    }

    [Fact]
    public void ImgIsTheShortFormOfImage()
    {
        Assert.Equal(Image("/logo.png").Render(), Img("/logo.png").Render());
        Assert.Equal("<img src=\"/logo.png\">", Img("/logo.png").Render());
        Assert.Equal("<img>", Img().Render());
    }

    [Fact]
    public void ListsReadTheWayTheMarkupDoes()
    {
        string html = Ul(
                Li(A("first").Href("/first")),
                Li(A("second").Href("/second")))
            .Class("items")
            .Render();

        Assert.Equal(
            "<ul class=\"items\"><li><a href=\"/first\">first</a></li>"
            + "<li><a href=\"/second\">second</a></li></ul>",
            html);
    }
}
