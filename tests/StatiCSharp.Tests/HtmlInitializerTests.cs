using System.Linq;
using StatiCSharp.HtmlComponents;
using Xunit;

namespace StatiCSharp.Tests;

/// <summary>
/// Covers the collection-initializer syntax for nesting children.
/// </summary>
public class HtmlInitializerTests
{
    [Fact]
    public void ChildrenCanBeWrittenAsACollectionInitializer()
    {
        string html = new Div
        {
            new H1("Title"),
            new Paragraph("Text"),
        }.Render();

        Assert.Equal("<div><h1>Title</h1><p>Text</p></div>", html);
    }

    [Fact]
    public void AStringInTheInitializerBecomesText()
    {
        string html = new Div { "plain" }.Render();

        Assert.Equal("<div>plain</div>", html);
    }

    [Fact]
    public void NestingGoesAsDeepAsNeededAndAttributesStillAttach()
    {
        string html = new Body
        {
            new Div
            {
                new Article
                {
                    new H1("Title"),
                    new Div { new Text("<p>Body</p>") }.Class("content"),
                },
            }.Class("wrapper"),
        }.Render();

        Assert.Equal(
            "<body><div class=\"wrapper\"><article><h1>Title</h1>"
            + "<div class=\"content\"><p>Body</p></div></article></div></body>",
            html);
    }

    [Fact]
    public void AllThreeStylesProduceTheSameHtml()
    {
        // The point of adding both: nobody has to migrate, and a theme can mix them.
        string initializer = new Div
        {
            new H1("Title"),
            new Paragraph("Text"),
        }.Render();

        string arguments = new Div(
            new H1("Title"),
            new Paragraph("Text")).Render();

        string chained = new Div()
            .Add(new H1("Title"))
            .Add(new Paragraph("Text"))
            .Render();

        Assert.Equal(arguments, initializer);
        Assert.Equal(chained, initializer);
    }

    [Fact]
    public void AnElementDoesNotPassAsASequenceOfItsChildren()
    {
        // Only the non-generic IEnumerable is implemented, so LINQ does not attach to an
        // element and it cannot be mistaken for a collection of its own content. If this
        // ever starts compiling, that guarantee is gone.
        var element = new Div { new H1("Title") };

        Assert.False(
            element is System.Collections.Generic.IEnumerable<StatiCSharp.Interfaces.IHtmlComponent>,
            "An element must not be an IEnumerable<IHtmlComponent>.");
    }

    [Fact]
    public void TheContentIsStillEnumerableForWhoeverNeedsIt()
    {
        var element = new Div { new H1("Title"), new Paragraph("Text") };

        int children = element.Cast<object>().Count();

        Assert.Equal(2, children);
    }
}
