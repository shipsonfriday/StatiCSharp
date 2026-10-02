using StatiCSharp.Interfaces;
using System.Collections;
using System.Globalization;
using System.Net;
using System.Text;

namespace StatiCSharp.HtmlComponents;

/// <summary>
/// A base class for all basic HTML elements.
/// <para>
/// Deriving from this is supported: override <see cref="TagName"/>, and
/// <see cref="VoidElement"/> if the element takes no content. It gives you no <c>Add</c>
/// though, so assign <see cref="Children"/> in a constructor or declare an <c>Add</c> of your
/// own if you want a collection initializer. <see cref="HtmlElement{TSelf}"/> is the usual
/// choice: it brings both, plus the shared fluent methods typed to your own element.
/// </para>
/// <para>
/// Implements the non-generic <see cref="IEnumerable"/> so that children can be written
/// as a collection initializer:
/// <code>
/// new Div { new H1("Title"), new Paragraph("Text") }
/// </code>
/// Deliberately not <c>IEnumerable&lt;IHtmlComponent&gt;</c>: that is more than the
/// initializer needs, and it would make an element pass as a sequence of its own
/// children wherever one is expected.
/// </para>
/// </summary>
public abstract class HtmlElement : IHtmlComponent, IEnumerable
{
    /// <summary>
    /// Defines the tagname of the element.
    /// </summary>
    protected abstract string TagName { get; }

    /// <summary>
    /// If this element is a void element.<br/>
    /// A void element is an element whose content model never allows it to have contents under any circumstances.<br/>
    /// Void elements can have attributes.<br/>
    /// Void elements only have a start tag. Closing tags must not be specified for void elements.
    /// </summary>
    protected virtual bool VoidElement { get; set; }

    /// <summary>
    /// The components inside this element.
    /// </summary>
    protected List<IHtmlComponent> Children { get; set; }

    /// <summary>
    /// Contains the attributes that are added to the opening tag of the element.
    /// &lt;Key&gt;&lt;Value&gt; is equivalent to Key="Value". A null value writes the name
    /// alone, as a boolean attribute like <c>checked</c> is written.
    /// <para>
    /// Ordered, and not an ordinary <see cref="Dictionary{TKey, TValue}"/>, because the
    /// order ends up in the output. A dictionary happens to enumerate in insertion order as
    /// long as nothing is removed, but that is an implementation detail its contract does
    /// not promise - and if it ever changed, every file of every generated website would be
    /// rewritten with its attributes shuffled. Setting a key that is already there keeps its
    /// position, so <c>Class("a").Id("b").Class("c")</c> still writes the class first.
    /// </para>
    /// </summary>
    protected OrderedDictionary<string, string?> Attributes { get; set; } = [];

    /// <summary>
    /// Initiate a new Html-Element, based on the derived class.
    /// </summary>
    public HtmlElement()
    {
        Children = new List<IHtmlComponent>();
    }

    /// <summary>
    /// Enumerates the components inside this element.
    /// <para>
    /// Present so that a collection initializer can be used; see <see cref="HtmlElement"/>.
    /// </para>
    /// </summary>
    /// <returns>An enumerator over the content of this element.</returns>
    IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();

    // No public Add here on purpose. It lives on HtmlElement<TSelf, TChild>, typed to what the
    // element may contain, so that an element with a content rule can state it. A public Add
    // on this class would be inherited by every element and let anything into anything - a Div
    // into a Ul, content into an Input that cannot render it.

    /// <summary>
    /// Rejects attribute names that would not survive being written into a tag.
    /// <para>
    /// Values are encoded when rendering, but names are not: a name containing a space
    /// or an equals sign would turn into further attributes. That only became reachable
    /// once callers could choose the name, e.g. building one from a tag out of the front
    /// matter.
    /// </para>
    /// </summary>
    protected static string CheckedKey(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        foreach (char character in key)
        {
            if (char.IsWhiteSpace(character) || char.IsControl(character)
                || character is '"' or '\'' or '>' or '/' or '=')
            {
                throw new ArgumentException(
                    $"\"{key}\" cannot be used as an attribute name: it contains '{character}'.",
                    nameof(key));
            }
        }

        return key;
    }

    /// <summary>
    /// Renders the element to html code.
    /// </summary>
    /// <returns>A string containing the html code of this element</returns>
    public virtual string Render()
    {
        StringBuilder elementBuilder = new();

        // Build leading tag
        elementBuilder.Append(CultureInfo.InvariantCulture, $"<{TagName}");

        // One pass, so the attributes are written in the order they were set. There used
        // to be two - first the ones with a value, then the ones without - which moved a
        // boolean attribute like "checked" behind every other attribute, wherever the
        // caller had put it.
        foreach (KeyValuePair<string, string?> attribute in Attributes)
        {
            if (string.IsNullOrEmpty(attribute.Key))
            {
                continue;
            }

            if (attribute.Value is null)
            {
                // The name stands alone, the way "checked" and "hidden" are written.
                elementBuilder.Append(CultureInfo.InvariantCulture, $" {attribute.Key}");
                continue;
            }

            // Encoded, so a quote in the value cannot end the attribute early and break
            // the tag. Values are taken literally, not as markup. An empty value is still
            // a value: aria-label="" and value="" say something, and dropping them
            // silently loses whatever the caller asked for.
            elementBuilder.Append(CultureInfo.InvariantCulture, $" {attribute.Key}=\"{WebUtility.HtmlEncode(attribute.Value)}\"");
        }

        // Close leading tag
        elementBuilder.Append('>');

        // Build content of the element
        if (!VoidElement)
        {
            foreach (IHtmlComponent element in Children)
            {
                elementBuilder.Append(element.Render());
            }

            // Build trailing tag
            elementBuilder.Append(CultureInfo.InvariantCulture, $"</{TagName}>");
        }

        return elementBuilder.ToString();
    }
}

/// <summary>
/// An element that accepts children of one type, with the fluent methods every element shares,
/// typed so that they hand back the derived element rather than the base.
/// <para>
/// Without <typeparamref name="TSelf"/> a chain would lose the derived type after the
/// first shared method, and this would not compile:
/// <code>
/// new A("Link").Class("nav").Href("/posts")
/// </code>
/// because <c>Class</c> would have returned <see cref="HtmlElement"/>, which has no
/// <c>Href</c>. The call order used to matter; it no longer does.
/// </para>
/// <para>
/// <typeparamref name="TChild"/> is for the places where the html standard says what may be
/// inside: a <c>ul</c> holds <c>li</c> elements, a <c>tbody</c> holds <c>tr</c> elements, a void
/// element holds nothing. Because <c>Add</c> lives here and not on <see cref="HtmlElement"/>,
/// naming the child type is the whole restriction - there is no wider <c>Add</c> to fall back
/// to, so a collection initializer cannot go around it either. Use
/// <see cref="HtmlElement{TSelf}"/> for an element that takes any content.
/// </para>
/// </summary>
/// <typeparam name="TSelf">The deriving element type.</typeparam>
/// <typeparam name="TChild">What this element may contain.</typeparam>
public abstract class HtmlElement<TSelf, TChild> : HtmlElement
    where TSelf : HtmlElement<TSelf, TChild>
    where TChild : IHtmlComponent
{
    /// <summary>
    /// Adds a child to the content of this element.
    /// <para>
    /// Returns nothing on purpose. Together with <see cref="IEnumerable"/> this is what a
    /// collection initializer needs, which ignores the return value:
    /// <code>
    /// new Div { new H1("Title"), new Paragraph("Text") }
    /// </code>
    /// It used to return the element so that calls could be chained. That third way of nesting
    /// was dropped in favour of the two above, and a void return is what makes it impossible
    /// rather than merely unused.
    /// </para>
    /// </summary>
    /// <param name="child">The child to add.</param>
    public void Add(TChild child) => Children.Add(child);

    /// <summary>
    /// Add a class attribute.
    /// </summary>
    /// <param name="cssClass">The name of the css class you want to assign.</param>
    /// <returns>this - the element itself, typed as <typeparamref name="TSelf"/>.</returns>
    public TSelf Class(string cssClass)
    {
        Attributes["class"] = cssClass;
        return (TSelf)this;
    }

    /// <summary>
    /// Add a style attribute.
    /// </summary>
    /// <param name="style">The content of the style attribute.</param>
    /// <returns>this - The element itself.></returns>
    public TSelf Style(string style)
    {
        Attributes["style"] = style;
        return (TSelf)this;
    }

    /// <summary>
    /// Specifies the width of the element.
    /// </summary>
    /// <param name="width"></param>
    /// <returns>this - the element itself, typed as <typeparamref name="TSelf"/>.</returns>
    public TSelf Width(int width)
    {
        Attributes["width"] = width.ToString(CultureInfo.InvariantCulture);
        return (TSelf)this;
    }

    /// <summary>
    /// Specifies the height of the element.
    /// </summary>
    /// <param name="height"></param>
    /// <returns>this - the element itself, typed as <typeparamref name="TSelf"/>.</returns>
    public TSelf Height(int height)
    {
        Attributes["height"] = height.ToString(CultureInfo.InvariantCulture);
        return (TSelf)this;
    }

    /// <summary>
    /// Indicates that the element is not yet, or is no longer, relevant.<br/>
    /// The browser won't render such elements. This attribute must not be used to hide content that could legitimately be shown.
    /// </summary>
    /// <returns></returns>
    public TSelf Hidden()
    {
        Attributes["hidden"] = null;
        return (TSelf)this;
    }

    /// <summary>
    /// Defines a unique identifier which must be unique in the whole document.<br/>
    /// Its purpose is to identify the element when linking, scripting, or styling.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public TSelf Id(string id)
    {
        Attributes["id"] = id;
        return (TSelf)this;
    }

    /// <summary>
    /// An integer attribute indicating if the element can take input focus, if it should participate to sequential keyboard navigation, and if so, at what position.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public TSelf TabIndex(int index)
    {
        Attributes["tabindex"] = index.ToString(CultureInfo.InvariantCulture);
        return (TSelf)this;
    }

    /// <summary>
    /// Sets an attribute that carries no value, e.g. <c>required</c> or <c>open</c>.
    /// </summary>
    /// <param name="key">The name of the attribute.</param>
    /// <returns>this - the element itself, typed as <typeparamref name="TSelf"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="key"/> is blank or not a usable attribute name.</exception>
    public TSelf Attribute(string key)
    {
        Attributes[CheckedKey(key)] = null;
        return (TSelf)this;
    }

    /// <summary>
    /// Sets any attribute, for the cases this library has no dedicated method for -
    /// <c>data-*</c>, <c>aria-*</c>, <c>role</c>, <c>rel</c> and the like.
    /// <para>
    /// The value is encoded when the element is rendered. Pass null to write the
    /// attribute without a value.
    /// </para>
    /// </summary>
    /// <param name="key">The name of the attribute.</param>
    /// <param name="value">The value of the attribute, or null for none.</param>
    /// <returns>this - the element itself, typed as <typeparamref name="TSelf"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="key"/> is blank or not a usable attribute name.</exception>
    public TSelf Attribute(string key, string? value)
    {
        Attributes[CheckedKey(key)] = value;
        return (TSelf)this;
    }

    /// <summary>
    /// Sets any attribute to a number, formatted invariantly so that the result does not
    /// depend on the machine's locale.
    /// </summary>
    /// <param name="key">The name of the attribute.</param>
    /// <param name="value">The value of the attribute.</param>
    /// <returns>this - the element itself, typed as <typeparamref name="TSelf"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="key"/> is blank or not a usable attribute name.</exception>
    public TSelf Attribute(string key, int value)
    {
        Attributes[CheckedKey(key)] = value.ToString(CultureInfo.InvariantCulture);
        return (TSelf)this;
    }
}

/// <summary>
/// An element that takes any content, which is most of them.
/// <para>
/// A shorthand for <see cref="HtmlElement{TSelf, TChild}"/> with no restriction, and what a
/// custom element in a theme derives from. It adds the <c>Add</c> for plain text, which an
/// element with a content rule must not have: a bare string is not an <c>li</c>.
/// </para>
/// </summary>
/// <typeparam name="TSelf">The deriving element type.</typeparam>
public abstract class HtmlElement<TSelf> : HtmlElement<TSelf, IHtmlComponent>
    where TSelf : HtmlElement<TSelf>
{
    /// <summary>
    /// Adds text to the content of this element. The text is written as it is; use encoding at
    /// the call site if it comes from anywhere but your own code.
    /// </summary>
    /// <param name="text">The text to add inside the content of the element.</param>
    public void Add(string text) => Children.Add(new Text(text));
}

/// <summary>
/// The content type of an element that can hold none. No instance of this can be made, so
/// <see cref="HtmlElement{TSelf, TChild}.Add(TChild)"/> has nothing to accept and the element
/// cannot be given content - not through a constructor, not through <c>Add</c>, not through a
/// collection initializer.
/// <para>
/// Before this, <c>new Input { new Div("lost") }</c> compiled and rendered <c>&lt;input&gt;</c>:
/// the content was accepted and dropped without a word, because a void element has no closing
/// tag to put it in.
/// </para>
/// </summary>
public sealed class NoContent : IHtmlComponent
{
    private NoContent()
    {
    }

    /// <summary>Never called: there is no instance to call it on.</summary>
    /// <returns>An empty string.</returns>
    public string Render() => string.Empty;
}
