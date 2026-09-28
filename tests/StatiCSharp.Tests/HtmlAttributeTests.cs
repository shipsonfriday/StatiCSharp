using System;
using System.Globalization;
using StatiCSharp.HtmlComponents;
using Xunit;

namespace StatiCSharp.Tests;

/// <summary>
/// Covers the general attribute escape hatch on every element.
/// </summary>
public class HtmlAttributeTests
{
    [Fact]
    public void AnyAttributeCanBeSet()
    {
        // Without this, a theme could not write data-, aria- or role attributes at all.
        string html = new Div("body")
            .Attribute("data-section", "posts")
            .Attribute("role", "region")
            .Render();

        Assert.Contains("data-section=\"posts\"", html, StringComparison.Ordinal);
        Assert.Contains("role=\"region\"", html, StringComparison.Ordinal);
    }

    [Fact]
    public void AnAttributeWithoutAValueIsWrittenAsABareName()
    {
        string html = new Div("body").Attribute("inert").Render();

        Assert.Equal("<div inert>body</div>", html);
    }

    [Fact]
    public void PassingNullAsTheValueAlsoWritesABareName()
    {
        Assert.Equal("<div inert>body</div>", new Div("body").Attribute("inert", null).Render());
    }

    [Fact]
    public void AnEmptyValueIsStillWritten()
    {
        // It used to fall through both render branches and vanish: not "value present",
        // not null either. aria-label="" and value="" both say something.
        Assert.Equal("<div data-flag=\"\">body</div>", new Div("body").Attribute("data-flag", "").Render());
    }

    [Fact]
    public void AnEmptyValueIsDistinctFromNoValue()
    {
        Assert.Equal("<div data-flag=\"\">body</div>", new Div("body").Attribute("data-flag", "").Render());
        Assert.Equal("<div data-flag>body</div>", new Div("body").Attribute("data-flag", null).Render());
    }

    [Fact]
    public void AnEmptyClassIsWrittenToo()
    {
        Assert.Equal("<div class=\"\">body</div>", new Div("body").Class("").Render());
    }

    [Fact]
    public void TheValueIsEncoded()
    {
        string html = new Div("body").Attribute("data-title", "The \"quoted\" & co").Render();

        Assert.Contains("data-title=\"The &quot;quoted&quot; &amp; co\"", html, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("de-DE")]
    [InlineData("sv-SE")]
    public void ANumericValueIsFormattedInvariantly(string culture)
    {
        // The overload exists so callers are not pushed into value.ToString(), which is
        // where the culture-dependent formatting bugs came from.
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);

            string html = new Div("body").Attribute("data-count", -1234).Render();

            Assert.Contains("data-count=\"-1234\"", html, StringComparison.Ordinal);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void ASetAttributeCanBeOverwritten()
    {
        string html = new Div("body").Attribute("role", "region").Attribute("role", "main").Render();

        Assert.Equal("<div role=\"main\">body</div>", html);
    }

    [Theory]
    [InlineData("has space")]
    [InlineData("has=equals")]
    [InlineData("has\"quote")]
    [InlineData("has'apostrophe")]
    [InlineData("has>bracket")]
    [InlineData("has/slash")]
    public void AnUnusableAttributeNameIsRejected(string key)
    {
        // Values are encoded, names are not. A name with a space or an equals sign would
        // turn into further attributes, which matters now that callers choose the name -
        // e.g. building one from a tag out of the front matter.
        var thrown = Assert.Throws<ArgumentException>(() => new Div("body").Attribute(key, "x"));

        Assert.Equal("key", thrown.ParamName);
    }

    [Fact]
    public void ABlankAttributeNameIsRejected()
    {
        Assert.Throws<ArgumentException>(() => new Div("body").Attribute("   ", "x"));
        Assert.Throws<ArgumentNullException>(() => new Div("body").Attribute(null!, "x"));
    }

    [Fact]
    public void InputAlsoRejectsAnUnusableName()
    {
        // Input declares its own overload, so it has to reach the same check rather than
        // writing straight into the attribute dictionary.
        var thrown = Assert.Throws<ArgumentException>(() => new Input().Attribute("has space", "x"));

        Assert.Equal("key", thrown.ParamName);
        Assert.Throws<ArgumentException>(() => new Input().Attribute("has space", 1));
    }

    [Fact]
    public void InputKeepsItsOwnOverloadAndStaysChainable()
    {
        // Input declares Attribute itself so that the chain keeps the Input type.
        string html = new Input().Type("number").Attribute("data-step", 5).Min(1.5f).Render();

        Assert.Contains("data-step=\"5\"", html, StringComparison.Ordinal);
        Assert.Contains("min=\"1.5\"", html, StringComparison.Ordinal);
    }
}
