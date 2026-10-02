using StatiCSharp.Interfaces;

namespace StatiCSharp.HtmlComponents;

/// <summary>
/// A representation of a &lt;form&gt;&lt;/form&gt; element.
/// <para>Call the Render() method to turn it into an HTML string.</para>
/// </summary>
public class Form : HtmlElement<Form>, IHtmlComponent
{
    /// <inheritdoc/>
    protected override string TagName => "form";

    /// <summary>Initiate a new and empty form element.</summary>
    public Form() { }

    /// <summary>Initiate a new form element with the given content.</summary>
    /// <param name="content">The elements or components inside the element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    public Form(params IHtmlComponent[] content)
    {
        ArgumentNullException.ThrowIfNull(content);

        Children = [.. content];
    }

    /// <summary>Initiate a new form element with the given text.</summary>
    /// <param name="text">The text inside the element.</param>
    public Form(string text)
    {
        Children = [new Text(text)];
    }

    /// <summary>Sets the <c>action</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Form Action(string value) => Attribute("action", value);

    /// <summary>Sets the <c>method</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Form Method(FormMethod value) => Attribute("method", AttributeValue.Of(value));

    /// <summary>Sets the <c>method</c> attribute to anything. Use the <see cref="FormMethod"/> overload otherwise.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Form Method(string value) => Attribute("method", value);
}

/// <summary>
/// A representation of a &lt;label&gt;&lt;/label&gt; element.
/// <para>Call the Render() method to turn it into an HTML string.</para>
/// </summary>
public class Label : HtmlElement<Label>, IHtmlComponent
{
    /// <inheritdoc/>
    protected override string TagName => "label";

    /// <summary>Initiate a new and empty label element.</summary>
    public Label() { }

    /// <summary>Initiate a new label element with the given content.</summary>
    /// <param name="content">The elements or components inside the element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    public Label(params IHtmlComponent[] content)
    {
        ArgumentNullException.ThrowIfNull(content);

        Children = [.. content];
    }

    /// <summary>Initiate a new label element with the given text.</summary>
    /// <param name="text">The text inside the element.</param>
    public Label(string text)
    {
        Children = [new Text(text)];
    }

    /// <summary>Sets the <c>for</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Label For(string value) => Attribute("for", value);
}

/// <summary>
/// A representation of a &lt;button&gt;&lt;/button&gt; element.
/// <para>Call the Render() method to turn it into an HTML string.</para>
/// </summary>
public class Button : HtmlElement<Button>, IHtmlComponent
{
    /// <inheritdoc/>
    protected override string TagName => "button";

    /// <summary>Initiate a new and empty button element.</summary>
    public Button() { }

    /// <summary>Initiate a new button element with the given content.</summary>
    /// <param name="content">The elements or components inside the element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    public Button(params IHtmlComponent[] content)
    {
        ArgumentNullException.ThrowIfNull(content);

        Children = [.. content];
    }

    /// <summary>Initiate a new button element with the given text.</summary>
    /// <param name="text">The text inside the element.</param>
    public Button(string text)
    {
        Children = [new Text(text)];
    }

    /// <summary>Sets the <c>type</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Button Type(ButtonType value) => Attribute("type", AttributeValue.Of(value));

    /// <summary>Sets the <c>type</c> attribute to anything. Use the <see cref="ButtonType"/> overload otherwise.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Button Type(string value) => Attribute("type", value);

    /// <summary>Sets the <c>disabled</c> attribute, which carries no value.</summary>
    /// <returns>this - the element itself.</returns>
    public Button Disabled() => Attribute("disabled");
}

/// <summary>
/// A representation of a &lt;select&gt;&lt;/select&gt; element.
/// <para>Call the Render() method to turn it into an HTML string.</para>
/// </summary>
public class Select : HtmlElement<Select>, IHtmlComponent
{
    /// <inheritdoc/>
    protected override string TagName => "select";

    /// <summary>Initiate a new and empty select element.</summary>
    public Select() { }

    /// <summary>Initiate a new select element with the given content.</summary>
    /// <param name="content">The elements or components inside the element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    public Select(params IHtmlComponent[] content)
    {
        ArgumentNullException.ThrowIfNull(content);

        Children = [.. content];
    }

    /// <summary>Initiate a new select element with the given text.</summary>
    /// <param name="text">The text inside the element.</param>
    public Select(string text)
    {
        Children = [new Text(text)];
    }

    /// <summary>Sets the <c>name</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Select Name(string value) => Attribute("name", value);

    /// <summary>Sets the <c>multiple</c> attribute, which carries no value.</summary>
    /// <returns>this - the element itself.</returns>
    public Select Multiple() => Attribute("multiple");
}

/// <summary>
/// A representation of a &lt;option&gt;&lt;/option&gt; element.
/// <para>Call the Render() method to turn it into an HTML string.</para>
/// </summary>
public class Option : HtmlElement<Option>, IHtmlComponent
{
    /// <inheritdoc/>
    protected override string TagName => "option";

    /// <summary>Initiate a new and empty option element.</summary>
    public Option() { }

    /// <summary>Initiate a new option element with the given content.</summary>
    /// <param name="content">The elements or components inside the element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    public Option(params IHtmlComponent[] content)
    {
        ArgumentNullException.ThrowIfNull(content);

        Children = [.. content];
    }

    /// <summary>Initiate a new option element with the given text.</summary>
    /// <param name="text">The text inside the element.</param>
    public Option(string text)
    {
        Children = [new Text(text)];
    }

    /// <summary>Sets the <c>value</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Option Value(string value) => Attribute("value", value);

    /// <summary>Sets the <c>selected</c> attribute, which carries no value.</summary>
    /// <returns>this - the element itself.</returns>
    public Option Selected() => Attribute("selected");
}

/// <summary>
/// A representation of a &lt;textarea&gt;&lt;/textarea&gt; element.
/// <para>Call the Render() method to turn it into an HTML string.</para>
/// </summary>
public class Textarea : HtmlElement<Textarea>, IHtmlComponent
{
    /// <inheritdoc/>
    protected override string TagName => "textarea";

    /// <summary>Initiate a new and empty textarea element.</summary>
    public Textarea() { }

    /// <summary>Initiate a new textarea element with the given content.</summary>
    /// <param name="content">The elements or components inside the element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    public Textarea(params IHtmlComponent[] content)
    {
        ArgumentNullException.ThrowIfNull(content);

        Children = [.. content];
    }

    /// <summary>Initiate a new textarea element with the given text.</summary>
    /// <param name="text">The text inside the element.</param>
    public Textarea(string text)
    {
        Children = [new Text(text)];
    }

    /// <summary>Sets the <c>name</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Textarea Name(string value) => Attribute("name", value);

    /// <summary>Sets the <c>rows</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Textarea Rows(int value) => Attribute("rows", value);

    /// <summary>Sets the <c>cols</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Textarea Cols(int value) => Attribute("cols", value);

    /// <summary>Sets the <c>placeholder</c> attribute.</summary>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself.</returns>
    public Textarea Placeholder(string value) => Attribute("placeholder", value);
}
