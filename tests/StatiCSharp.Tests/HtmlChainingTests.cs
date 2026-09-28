using System;
using StatiCSharp.HtmlComponents;
using Xunit;

namespace StatiCSharp.Tests;

/// <summary>
/// Covers that a fluent chain keeps the derived element type.
/// <para>
/// Most of these are really compile-time assertions: before HtmlElement gained its TSelf
/// parameter, the shared methods returned the base type, so a type-specific method after
/// one of them did not compile. If that regresses, this file stops building.
/// </para>
/// </summary>
public class HtmlChainingTests
{
    [Fact]
    public void ATypeSpecificMethodCanFollowASharedOne()
    {
        // This is the case that used to fail with
        // "error CS1061: 'HtmlElement' does not contain a definition for 'Href'".
        string html = new A("Link").Class("nav").Href("/posts").Render();

        Assert.Contains("href=\"/posts\"", html, StringComparison.Ordinal);
        Assert.Contains("class=\"nav\"", html, StringComparison.Ordinal);
    }

    [Fact]
    public void TheCallOrderNoLongerMatters()
    {
        string typeFirst = new A("Link").Href("/posts").Class("nav").Render();
        string sharedFirst = new A("Link").Class("nav").Href("/posts").Render();

        // Only the attribute order in the output differs, the content does not.
        foreach (string fragment in new[] { "href=\"/posts\"", "class=\"nav\"", ">Link</a>" })
        {
            Assert.Contains(fragment, typeFirst, StringComparison.Ordinal);
            Assert.Contains(fragment, sharedFirst, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void TheDerivedTypeSurvivesALongChain()
    {
        A link = new A("Link")
            .Class("nav")
            .Id("main-nav")
            .Attribute("data-kind", "section")
            .TabIndex(0)
            .Style("color: red")
            .Href("/posts")
            .Target("_blank");

        Assert.Contains("target=\"_blank\"", link.Render(), StringComparison.Ordinal);
    }

    [Fact]
    public void ListItemsKeepTheirOwnMethodsToo()
    {
        Li item = new Li("first").Class("item").Value("3");

        Assert.Contains("value=\"3\"", item.Render(), StringComparison.Ordinal);
    }

    [Fact]
    public void InputKeepsItsTypeThroughTheSharedMethods()
    {
        // Input used to redeclare Attribute just to keep the chain on Input. It inherits a
        // correctly typed one now, and the redeclarations are gone.
        Input field = new Input()
            .Class("field")
            .Attribute("data-role", "search")
            .Type("number")
            .Min(1.5f);

        string html = field.Render();

        Assert.Contains("type=\"number\"", html, StringComparison.Ordinal);
        Assert.Contains("min=\"1.5\"", html, StringComparison.Ordinal);
        Assert.Contains("data-role=\"search\"", html, StringComparison.Ordinal);
    }

    [Fact]
    public void AddKeepsTheDerivedTypeAsWell()
    {
        Div wrapper = new Div()
            .Add(new H1("Title"))
            .Class("wrapper")
            .Add(new Paragraph("Text"));

        Assert.Equal(
            "<div class=\"wrapper\"><h1>Title</h1><p>Text</p></div>",
            wrapper.Render());
    }
}
