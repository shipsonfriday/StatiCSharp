using StatiCSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace StatiCSharp.HtmlComponents
{
    /// <summary>
    /// A representation of a &lt;nav&gt;&lt;/nav&gt; element.
    /// Call the Render() method to turn it into an HTML string.
    /// </summary>
    public class Nav : HtmlElement<Nav>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName
        {
            get { return "nav"; }
        }

        /// <summary>
        /// Initiate a new empty nav element.
        /// </summary>
        public Nav()
        {
            // No action needed, because the base class already initialized an empty List<IHtmlComponent>.
        }

        /// <summary>
        /// Initiate a new nav element with another element or component inside.
        /// </summary>
        /// <param name="component">The element or component for the content of the nav tag.</param>
        public Nav(IHtmlComponent component)
        {
            Content = new List<IHtmlComponent> { component };
        }

        /// <summary>
        /// Initiate a new nav element with text.
        /// </summary>
        /// <param name="text">The text for the content of the nav tag.</param>
        public Nav(string text)
        {
            Content = new List<IHtmlComponent> { new Text(text) };
        }

        /// <summary>
        /// Initiate a new nav element with the given content. Lets elements be nested by
        /// passing their children as arguments.
        /// </summary>
        /// <param name="content">The elements or components for the content of the nav element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Nav(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Content = [.. content];
        }

    }
}
