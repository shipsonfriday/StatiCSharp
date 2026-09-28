using System;
using StatiCSharp.HtmlComponents;
using StatiCSharp.Interfaces;
using Xunit;

namespace StatiCSharp.Tests;

/// <summary>
/// An element the library does not ship, defined outside its assembly.
/// Named SectionTag rather than Section so it does not shadow the site model type.
/// <para>
/// making_a_custom_theme.md has always claimed this is possible. It was not: TagName was
/// private protected abstract, so an external subclass could neither see it nor omit it.
/// This class existing at all is the assertion.
/// </para>
/// </summary>
public class SectionTag : HtmlElement<SectionTag>
{
    protected override string TagName => "section";

    public SectionTag() { }

    public SectionTag(params IHtmlComponent[] content)
    {
        Children = [.. content];
    }

    public SectionTag(string text)
    {
        Children = [new Text(text)];
    }
}

/// <summary>
/// A void element, to check that <see cref="HtmlElement.VoidElement"/> is reachable too.
/// </summary>
public class HorizontalRule : HtmlElement<HorizontalRule>
{
    protected override string TagName => "hr";

    protected override bool VoidElement => true;
}

/// <summary>
/// Derived from the non-generic base, for an element that needs no fluent chain.
/// </summary>
public class Marker : HtmlElement
{
    protected override string TagName => "mark";
}

public class CustomElementTests
{
    [Fact]
    public void ACustomElementRenders()
    {
        Assert.Equal("<section>body</section>", new SectionTag("body").Render());
    }

    [Fact]
    public void ACustomElementGetsTheSharedFluentMethodsTypedToItself()
    {
        SectionTag section = new SectionTag(new H1("Title"))
            .Class("content")
            .Attribute("aria-label", "Main");

        Assert.Equal(
            "<section class=\"content\" aria-label=\"Main\"><h1>Title</h1></section>",
            section.Render());
    }

    [Fact]
    public void ACustomElementWorksInTheNestingAndInitializerSyntax()
    {
        string nested = new Div(new SectionTag(new Paragraph("Text"))).Render();
        string initializer = new Div { new SectionTag { new Paragraph("Text") } }.Render();

        Assert.Equal("<div><section><p>Text</p></section></div>", nested);
        Assert.Equal(nested, initializer);
    }

    [Fact]
    public void ACustomVoidElementOmitsItsClosingTag()
    {
        Assert.Equal("<hr class=\"divider\">", new HorizontalRule().Class("divider").Render());
    }

    [Fact]
    public void TheNonGenericBaseCanBeDerivedFromAsWell()
    {
        Assert.Equal("<mark></mark>", new Marker().Render());
    }

    [Fact]
    public void TheInitializerWorksOnTheNonGenericBaseToo()
    {
        // Add and IEnumerable both sit on the non-generic base, so the initializer is
        // available even without the TSelf parameter.
        Assert.Equal("<mark><h1>Title</h1></mark>", new Marker { new H1("Title") }.Render());
    }

    [Fact]
    public void ACustomElementGetsTheAttributeNameCheckingToo()
    {
        Assert.Throws<ArgumentException>(() => new SectionTag().Attribute("has space", "x"));
    }
}
