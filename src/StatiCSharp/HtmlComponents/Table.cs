using StatiCSharp.Interfaces;

namespace StatiCSharp.HtmlComponents;

/// <summary>
/// A cell of a table row, which the html standard says is a <c>td</c> or a <c>th</c> and nothing
/// else. The two have nothing else in common, so this says only that - it is what lets
/// <see cref="Tr"/> name what it may contain.
/// </summary>
public interface ITableCell : IHtmlComponent
{
}

/// <summary>
/// A representation of a &lt;table&gt;&lt;/table&gt; element.
/// <para>Call the Render() method to turn it into an HTML string.</para>
/// </summary>
public class Table : HtmlElement<Table>, IHtmlComponent
{
    /// <inheritdoc/>
    protected override string TagName => "table";

    /// <summary>Initiate a new and empty table element.</summary>
    public Table() { }

    /// <summary>Initiate a new table element with the given content.</summary>
    /// <param name="content">The elements or components inside the element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    public Table(params IHtmlComponent[] content)
    {
        ArgumentNullException.ThrowIfNull(content);

        Children = [.. content];
    }

    /// <summary>Initiate a new table element with the given text.</summary>
    /// <param name="text">The text inside the element.</param>
    public Table(string text)
    {
        Children = [new Text(text)];
    }
}

/// <summary>
/// A representation of a &lt;caption&gt;&lt;/caption&gt; element.
/// <para>Call the Render() method to turn it into an HTML string.</para>
/// </summary>
public class Caption : HtmlElement<Caption>, IHtmlComponent
{
    /// <inheritdoc/>
    protected override string TagName => "caption";

    /// <summary>Initiate a new and empty caption element.</summary>
    public Caption() { }

    /// <summary>Initiate a new caption element with the given content.</summary>
    /// <param name="content">The elements or components inside the element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    public Caption(params IHtmlComponent[] content)
    {
        ArgumentNullException.ThrowIfNull(content);

        Children = [.. content];
    }

    /// <summary>Initiate a new caption element with the given text.</summary>
    /// <param name="text">The text inside the element.</param>
    public Caption(string text)
    {
        Children = [new Text(text)];
    }
}

/// <summary>
/// A representation of a &lt;thead&gt;&lt;/thead&gt; element.
/// <para>Call the Render() method to turn it into an HTML string.</para>
/// </summary>
public class Thead : HtmlElement<Thead, Tr>, IHtmlComponent
{
    /// <inheritdoc/>
    protected override string TagName => "thead";

    /// <summary>Initiate a new and empty thead element.</summary>
    public Thead() { }

    /// <summary>Initiate a new thead element with the given content.</summary>
    /// <param name="content">The elements or components inside the element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    public Thead(params Tr[] content)
    {
        ArgumentNullException.ThrowIfNull(content);

        Children = [.. content];
    }
}

/// <summary>
/// A representation of a &lt;tbody&gt;&lt;/tbody&gt; element.
/// <para>Call the Render() method to turn it into an HTML string.</para>
/// </summary>
public class Tbody : HtmlElement<Tbody, Tr>, IHtmlComponent
{
    /// <inheritdoc/>
    protected override string TagName => "tbody";

    /// <summary>Initiate a new and empty tbody element.</summary>
    public Tbody() { }

    /// <summary>Initiate a new tbody element with the given content.</summary>
    /// <param name="content">The elements or components inside the element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    public Tbody(params Tr[] content)
    {
        ArgumentNullException.ThrowIfNull(content);

        Children = [.. content];
    }
}

/// <summary>
/// A representation of a &lt;tfoot&gt;&lt;/tfoot&gt; element.
/// <para>Call the Render() method to turn it into an HTML string.</para>
/// </summary>
public class Tfoot : HtmlElement<Tfoot, Tr>, IHtmlComponent
{
    /// <inheritdoc/>
    protected override string TagName => "tfoot";

    /// <summary>Initiate a new and empty tfoot element.</summary>
    public Tfoot() { }

    /// <summary>Initiate a new tfoot element with the given content.</summary>
    /// <param name="content">The elements or components inside the element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    public Tfoot(params Tr[] content)
    {
        ArgumentNullException.ThrowIfNull(content);

        Children = [.. content];
    }
}

/// <summary>
/// A representation of a &lt;tr&gt;&lt;/tr&gt; element.
/// <para>Call the Render() method to turn it into an HTML string.</para>
/// </summary>
public class Tr : HtmlElement<Tr, ITableCell>, IHtmlComponent
{
    /// <inheritdoc/>
    protected override string TagName => "tr";

    /// <summary>Initiate a new and empty tr element.</summary>
    public Tr() { }

    /// <summary>Initiate a new tr element with the given content.</summary>
    /// <param name="content">The elements or components inside the element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    public Tr(params ITableCell[] content)
    {
        ArgumentNullException.ThrowIfNull(content);

        Children = [.. content];
    }
}

/// <summary>
/// A representation of a &lt;th&gt;&lt;/th&gt; element.
/// <para>Call the Render() method to turn it into an HTML string.</para>
/// </summary>
public class Th : HtmlElement<Th>, ITableCell
{
    /// <inheritdoc/>
    protected override string TagName => "th";

    /// <summary>Initiate a new and empty th element.</summary>
    public Th() { }

    /// <summary>Initiate a new th element with the given content.</summary>
    /// <param name="content">The elements or components inside the element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    public Th(params IHtmlComponent[] content)
    {
        ArgumentNullException.ThrowIfNull(content);

        Children = [.. content];
    }

    /// <summary>Initiate a new th element with the given text.</summary>
    /// <param name="text">The text inside the element.</param>
    public Th(string text)
    {
        Children = [new Text(text)];
    }

    /// <summary>Sets the <c>scope</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Th Scope(CellScope value) => Attribute("scope", AttributeValue.Of(value));

    /// <summary>Sets the <c>scope</c> attribute to anything. Use the <see cref="CellScope"/> overload otherwise.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Th Scope(string value) => Attribute("scope", value);

    /// <summary>Sets the <c>colspan</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Th ColSpan(int value) => Attribute("colspan", value);

    /// <summary>Sets the <c>rowspan</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Th RowSpan(int value) => Attribute("rowspan", value);
}

/// <summary>
/// A representation of a &lt;td&gt;&lt;/td&gt; element.
/// <para>Call the Render() method to turn it into an HTML string.</para>
/// </summary>
public class Td : HtmlElement<Td>, ITableCell
{
    /// <inheritdoc/>
    protected override string TagName => "td";

    /// <summary>Initiate a new and empty td element.</summary>
    public Td() { }

    /// <summary>Initiate a new td element with the given content.</summary>
    /// <param name="content">The elements or components inside the element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    public Td(params IHtmlComponent[] content)
    {
        ArgumentNullException.ThrowIfNull(content);

        Children = [.. content];
    }

    /// <summary>Initiate a new td element with the given text.</summary>
    /// <param name="text">The text inside the element.</param>
    public Td(string text)
    {
        Children = [new Text(text)];
    }

    /// <summary>Sets the <c>colspan</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Td ColSpan(int value) => Attribute("colspan", value);

    /// <summary>Sets the <c>rowspan</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Td RowSpan(int value) => Attribute("rowspan", value);
}
