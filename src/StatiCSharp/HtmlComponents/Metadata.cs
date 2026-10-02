using StatiCSharp.Interfaces;

namespace StatiCSharp.HtmlComponents;

/// <summary>
/// A representation of a &lt;link&gt; element.
/// <para>Call the Render() method to turn it into an HTML string.</para>
/// </summary>
public class Link : HtmlElement<Link>, IHtmlComponent
{
    /// <inheritdoc/>
    protected override string TagName => "link";

    /// <inheritdoc/>
    protected override bool VoidElement => true;

    /// <summary>Initiate a new link element.</summary>
    public Link() {{ }}

    /// <summary>Sets the <c>rel</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Link Rel(string value) => Attribute("rel", value);

    /// <summary>Sets the <c>href</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Link Href(string value) => Attribute("href", value);

    /// <summary>Sets the <c>type</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Link Type(string value) => Attribute("type", value);

    /// <summary>Sets the <c>sizes</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Link Sizes(string value) => Attribute("sizes", value);
}

/// <summary>
/// A representation of a &lt;meta&gt; element.
/// <para>Call the Render() method to turn it into an HTML string.</para>
/// </summary>
public class Meta : HtmlElement<Meta>, IHtmlComponent
{
    /// <inheritdoc/>
    protected override string TagName => "meta";

    /// <inheritdoc/>
    protected override bool VoidElement => true;

    /// <summary>Initiate a new meta element.</summary>
    public Meta() {{ }}

    /// <summary>Sets the <c>name</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Meta Name(string value) => Attribute("name", value);

    /// <summary>Sets the <c>content</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Meta Content(string value) => Attribute("content", value);

    /// <summary>Sets the <c>charset</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Meta Charset(string value) => Attribute("charset", value);
}

/// <summary>
/// A representation of a &lt;script&gt;&lt;/script&gt; element.
/// <para>Call the Render() method to turn it into an HTML string.</para>
/// </summary>
public class Script : HtmlElement<Script>, IHtmlComponent
{
    /// <inheritdoc/>
    protected override string TagName => "script";

    /// <summary>Initiate a new and empty script element.</summary>
    public Script() { }

    /// <summary>Initiate a new script element with the given content.</summary>
    /// <param name="content">The elements or components inside the element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    public Script(params IHtmlComponent[] content)
    {
        ArgumentNullException.ThrowIfNull(content);

        Children = [.. content];
    }

    /// <summary>Initiate a new script element with the given text.</summary>
    /// <param name="text">The text inside the element.</param>
    public Script(string text)
    {
        Children = [new Text(text)];
    }

    /// <summary>Sets the <c>src</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Script Src(string value) => Attribute("src", value);

    /// <summary>Sets the <c>type</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Script Type(string value) => Attribute("type", value);

    /// <summary>Sets the <c>async</c> attribute, which carries no value.</summary>
    /// <returns>this - the element itself.</returns>
    public Script Async() => Attribute("async");

    /// <summary>Sets the <c>defer</c> attribute, which carries no value.</summary>
    /// <returns>this - the element itself.</returns>
    public Script Defer() => Attribute("defer");
}

/// <summary>
/// A representation of a &lt;style&gt;&lt;/style&gt; element.
/// <para>Call the Render() method to turn it into an HTML string.</para>
/// </summary>
public class Style : HtmlElement<Style>, IHtmlComponent
{
    /// <inheritdoc/>
    protected override string TagName => "style";

    /// <summary>Initiate a new and empty style element.</summary>
    public Style() { }

    /// <summary>Initiate a new style element with the given content.</summary>
    /// <param name="content">The elements or components inside the element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    public Style(params IHtmlComponent[] content)
    {
        ArgumentNullException.ThrowIfNull(content);

        Children = [.. content];
    }

    /// <summary>Initiate a new style element with the given text.</summary>
    /// <param name="text">The text inside the element.</param>
    public Style(string text)
    {
        Children = [new Text(text)];
    }

    /// <summary>Sets the <c>media</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Style Media(string value) => Attribute("media", value);
}
