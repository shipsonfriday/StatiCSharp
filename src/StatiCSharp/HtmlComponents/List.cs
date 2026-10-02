using StatiCSharp.Interfaces;
using System.Globalization;

namespace StatiCSharp.HtmlComponents;

/// <summary>
/// A representation of a &lt;li&gt;&lt;/li&gt; element.
/// Call the Render() method to turn it into an HTML string.
/// </summary>
public class Li: HtmlElement<Li>, IHtmlComponent
{
    /// <inheritdoc/>
    protected override string TagName
    {
        get { return "li"; }
    }

    /// <summary>
    /// Initiate a new empty li element.
    /// </summary>
    public Li()
    {
        // No action needed, because the base class already initialized an empty List<IHtmlComponent>.
    }

    /// <summary>
    /// Initiate a new li element with another element or component inside.
    /// </summary>
    /// <param name="component">The component for the content of the li.</param>
    public Li(IHtmlComponent component)
    {
        Children = new List<IHtmlComponent> { component };
    }

    /// <summary>
    /// Initiate a new li element with text.
    /// </summary>
    /// <param name="text">The text inside the li element.</param>
    public Li(string text)
    {
        Children = new List<IHtmlComponent> { new Text(text) };
    }

    /// <summary>
    /// Initiate a new li element with the given content. Lets elements be nested by
    /// passing their children as arguments.
    /// </summary>
    /// <param name="content">The elements or components for the content of the li element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    public Li(params IHtmlComponent[] content)
    {
        ArgumentNullException.ThrowIfNull(content);

        Children = [.. content];
    }

    /// <summary>
    /// Only for &lt;ol&gt; lists.<br/>
    /// Specifies the start value of a list item. The following list items will increment from that number
    /// </summary>
    /// <param name="value"></param>
    /// <returns>this - the element itself.</returns>
    public Li Value(string value)
    {
        if (!Attributes.TryAdd("value", value))
        {
            Attributes["value"] = value;
        }
        return this;
    }

}


/// <summary>
/// A representation of a &lt;ul&gt;&lt;/ul&gt; element.
/// Call the Render() method to turn it into an HTML string.
/// </summary>
public class Ul : HtmlElement<Ul>, IHtmlComponent
{
    /// <inheritdoc/>
    protected override string TagName
    {
        get { return "ul"; }
    }

    /// <summary>
    /// Initiate a new empty &lt;ul&gt; element.
    /// </summary>
    public Ul()
    {
        // No action needed, because the base class already initialized an empty List<IHtmlComponent>.
    }

    /// <summary>
    /// Initiate a new &lt;ul&gt; element with another element or component inside.
    /// </summary>
    /// <param name="component">The component for the content of the ul.</param>
    public Ul(IHtmlComponent component)
    {
        Children = new List<IHtmlComponent> { component };
    }

    /// <summary>
    /// Initiate a new ul element with text.
    /// </summary>
    /// <param name="text">The text for the content of the &lt;ul&gt;.</param>
    public Ul(string text)
    {
        Children = new List<IHtmlComponent> { new Text(text) };
    }

    /// <summary>
    /// Initiate a new ul element with the given content. Lets elements be nested by
    /// passing their children as arguments.
    /// </summary>
    /// <param name="content">The elements or components for the content of the ul element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    public Ul(params IHtmlComponent[] content)
    {
        ArgumentNullException.ThrowIfNull(content);

        Children = [.. content];
    }

}


/// <summary>
/// A representation of a &lt;ol&gt;&lt;/ol&gt; element.<br/>
/// Call the Render() method to turn it into an HTML string.
/// </summary>
public class Ol : HtmlElement<Ol>, IHtmlComponent
{
    /// <inheritdoc/>
    protected override string TagName
    {
        get { return "ol"; }
    }

    /// <summary>
    /// Initiate a new empty &lt;ol&gt; element.
    /// </summary>
    public Ol()
    {
        // No action needed, because the base class already initialized an empty List<IHtmlComponent>.
    }

    /// <summary>
    /// Initiate a new &lt;ol&gt; element with another element or component inside.
    /// </summary>
    /// <param name="component">The component for the content of the ol.</param>
    public Ol(IHtmlComponent component)
    {
        Children = new List<IHtmlComponent> { component };
    }

    /// <summary>
    /// Initiate a new ol element with text.
    /// </summary>
    /// <param name="text">The text for the content of the &lt;ol&gt;.</param>
    public Ol(string text)
    {
        Children = new List<IHtmlComponent> { new Text(text) };
    }

    /// <summary>
    /// Initiate a new ol element with the given content. Lets elements be nested by
    /// passing their children as arguments.
    /// </summary>
    /// <param name="content">The elements or components for the content of the ol element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    public Ol(params IHtmlComponent[] content)
    {
        ArgumentNullException.ThrowIfNull(content);

        Children = [.. content];
    }

    /// <summary>
    /// The type attribute of the &lt;ol&gt; tag, defines the type of the list item marker:
    /// type="1"	The list items will be numbered with numbers (default)
    /// type="A"	The list items will be numbered with uppercase letters
    /// type="a"	The list items will be numbered with lowercase letters
    /// type="I"	The list items will be numbered with uppercase roman numbers
    /// type="i"	The list items will be numbered with lowercase roman numbers
    /// </summary>
    /// <param name="t">The type</param>
    /// <returns>this - the element itself.</returns>
    public Ol Type(char t)
    {
        if (!Attributes.TryAdd("type", t.ToString(CultureInfo.InvariantCulture)))
        {
            Attributes["type"] = t.ToString(CultureInfo.InvariantCulture);
        }
        return this;
    }

    /// <summary>
    /// The type attribute of the &lt;ol&gt; tag, defines the type of the list item marker:
    /// type="1"	The list items will be numbered with numbers (default)
    /// type="A"	The list items will be numbered with uppercase letters
    /// type="a"	The list items will be numbered with lowercase letters
    /// type="I"	The list items will be numbered with uppercase roman numbers
    /// type="i"	The list items will be numbered with lowercase roman numbers
    /// </summary>
    /// <param name="t">The type</param>
    /// <returns>this - the element itself.</returns>
    public Ol Type(int t)
    {
        if (!Attributes.TryAdd("type", t.ToString(CultureInfo.InvariantCulture)))
        {
            Attributes["type"] = t.ToString(CultureInfo.InvariantCulture);
        }
        return this;
    }

}
