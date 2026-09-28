using StatiCSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace StatiCSharp.HtmlComponents
{
    /// <summary>
    /// A representation of a &lt;span&gt;&lt;/span&gt; element.
    /// Call the Render() method to turn it into an HTML string.
    /// </summary>
    public class Span : HtmlElement<Span>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName
        {
            get { return "span"; }
        }

        /// <summary>
        /// Initiate a new empty &lt;span&gt; element.
        /// </summary>
        public Span()
        {
            // No action needed, because the base class already initialized an empty List<IHtmlComponent>.
        }

        /// <summary>
        /// Initiate a new &lt;span&gt; element with another element or component inside.
        /// </summary>
        /// <param name="component">The element or component for the content of the span.</param>
        public Span(IHtmlComponent component)
        {
            Children = new List<IHtmlComponent>() { component };
        }

        /// <summary>
        /// Initiate a new &lt;span&gt; element with text.
        /// </summary>
        /// <param name="text">The text for the content of the span.</param>
        public Span(string text)
        {
            Children = new List<IHtmlComponent>() { new Text(text) };
        }

        /// <summary>
        /// Initiate a new span with the given content. Lets elements be nested by
        /// passing their children as arguments.
        /// </summary>
        /// <param name="content">The elements or components for the content of the span.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Span(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

    }
}
