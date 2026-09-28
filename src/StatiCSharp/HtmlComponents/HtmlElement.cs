using StatiCSharp.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;

namespace StatiCSharp.HtmlComponents
{
    /// <summary>
    /// A base class for all basic HTML elements.
    /// <para>
    /// Deriving from this is supported: override <see cref="TagName"/>, and
    /// <see cref="VoidElement"/> if the element takes no content. Derive from
    /// <see cref="HtmlElement{TSelf}"/> instead to also get the shared fluent methods typed
    /// to your own element.
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
        /// Contains the components inside the element.
        /// </summary>
        protected List<IHtmlComponent> Content { get; set; }

        /// <summary>
        /// Contains the attributes that are added to the opening tag of the element.
        /// &lt;Key&gt;&lt;Value&gt; is equivalent to Key="Value".
        /// </summary>
        protected Dictionary<string, string?> Attributes { get; set; } = new Dictionary<string, string?>();

        /// <summary>
        /// Initiate a new Html-Element, based on the derived class.
        /// </summary>
        public HtmlElement()
        {
            Content = new List<IHtmlComponent>();
        }

        /// <summary>
        /// Enumerates the components inside this element.
        /// <para>
        /// Present so that a collection initializer can be used; see <see cref="HtmlElement"/>.
        /// </para>
        /// </summary>
        /// <returns>An enumerator over the content of this element.</returns>
        IEnumerator IEnumerable.GetEnumerator() => Content.GetEnumerator();

        /// <summary>
        /// Adds an element or component to the content of this element.
        /// <para>
        /// Returns nothing on purpose. Together with <see cref="IEnumerable"/> this is what
        /// a collection initializer needs, which ignores the return value:
        /// <code>
        /// new Div { new H1("Title"), new Paragraph("Text") }
        /// </code>
        /// It used to return the element so that calls could be chained. That third way of
        /// nesting was dropped in favour of the two above, and a void return is what makes
        /// it impossible rather than merely unused.
        /// </para>
        /// </summary>
        /// <param name="component">The element or component to add. Must implement IHtmlComponent.</param>
        public void Add(IHtmlComponent component) => Content.Add(component);

        /// <summary>
        /// Adds text to the content of this element. The text is written as it is; use
        /// encoding at the call site if it comes from anywhere but your own code.
        /// </summary>
        /// <param name="text">The text to add inside the content of the element.</param>
        public void Add(string text) => Content.Add(new Text(text));

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

            // Add attributes with key-value pairs. An empty value is still a value:
            // aria-label="" and value="" say something, and dropping them silently loses
            // whatever the caller asked for. Only null means "write the name alone".
            foreach (KeyValuePair<string, string?> attribute in Attributes)
            {
                if (!string.IsNullOrEmpty(attribute.Key) && attribute.Value is not null)
                {
                    // Encoded, so a quote in the value cannot end the attribute early
                    // and break the tag. Values are taken literally, not as markup.
                    elementBuilder.Append(CultureInfo.InvariantCulture, $" {attribute.Key}=\"{WebUtility.HtmlEncode(attribute.Value)}\"");
                }
            }

            // Add attributes with just keys
            foreach (KeyValuePair<string, string?> attribute in Attributes)
            {
                if ((!string.IsNullOrEmpty(attribute.Key)) && (attribute.Value is null))
                {
                    elementBuilder.Append(CultureInfo.InvariantCulture, $" {attribute.Key}");
                }
            }

            // Close leading tag
            elementBuilder.Append('>');

            // Build content of the element
            if (!VoidElement)
            {
                foreach (IHtmlComponent element in Content)
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
    /// Adds the fluent methods that every element shares, typed so that they hand back the
    /// derived element rather than the base.
    /// <para>
    /// Without <typeparamref name="TSelf"/> a chain would lose the derived type after the
    /// first shared method, and this would not compile:
    /// <code>
    /// new A("Link").Class("nav").Href("/posts")
    /// </code>
    /// because <c>Class</c> would have returned <see cref="HtmlElement"/>, which has no
    /// <c>Href</c>. The call order used to matter; it no longer does.
    /// </para>
    /// </summary>
    /// <typeparam name="TSelf">The deriving element type.</typeparam>
    public abstract class HtmlElement<TSelf> : HtmlElement
        where TSelf : HtmlElement<TSelf>
    {
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

}
