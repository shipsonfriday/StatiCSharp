using StatiCSharp.Interfaces;

namespace StatiCSharp.HtmlComponents;

/// <summary>
/// A representation of a &lt;details&gt;&lt;/details&gt; element.
/// <para>Call the Render() method to turn it into an HTML string.</para>
/// </summary>
public class Details : HtmlElement<Details>, IHtmlComponent
{
    /// <inheritdoc/>
    protected override string TagName => "details";

    /// <summary>Initiate a new and empty details element.</summary>
    public Details() { }

    /// <summary>Initiate a new details element with the given content.</summary>
    /// <param name="content">The elements or components inside the element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    public Details(params IHtmlComponent[] content)
    {
        ArgumentNullException.ThrowIfNull(content);

        Children = [.. content];
    }

    /// <summary>Initiate a new details element with the given text.</summary>
    /// <param name="text">The text inside the element.</param>
    public Details(string text)
    {
        Children = [new Text(text)];
    }

    /// <summary>Sets the <c>open</c> attribute, which carries no value.</summary>
    /// <returns>this - the element itself.</returns>
    public Details Open() => Attribute("open");
}

/// <summary>
/// A representation of a &lt;summary&gt;&lt;/summary&gt; element.
/// <para>Call the Render() method to turn it into an HTML string.</para>
/// </summary>
public class Summary : HtmlElement<Summary>, IHtmlComponent
{
    /// <inheritdoc/>
    protected override string TagName => "summary";

    /// <summary>Initiate a new and empty summary element.</summary>
    public Summary() { }

    /// <summary>Initiate a new summary element with the given content.</summary>
    /// <param name="content">The elements or components inside the element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    public Summary(params IHtmlComponent[] content)
    {
        ArgumentNullException.ThrowIfNull(content);

        Children = [.. content];
    }

    /// <summary>Initiate a new summary element with the given text.</summary>
    /// <param name="text">The text inside the element.</param>
    public Summary(string text)
    {
        Children = [new Text(text)];
    }
}
