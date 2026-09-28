using StatiCSharp.Interfaces;
using System;
using System.Collections.Generic;

namespace StatiCSharp.HtmlComponents
{
    /// <summary>
    /// A representation of a &lt;footer&gt;&lt;/footer&gt; element.
    /// Call the Render() method to turn it into an HTML string.
    /// </summary>
    public class Footer : HtmlElement<Footer>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName
        {
            get { return "footer"; }
        }

        /// <summary>
        /// Initiate a new empty &lt;footer&gt; element.
        /// </summary>
        public Footer()
        {
            // No action needed, because the base class already initialized an empty List<IHtmlComponent>.
        }

        /// <summary>
        /// Initiate a new &lt;footer&gt; with another element or component inside.
        /// </summary>
        /// <param name="component">The element or component for the content of the &lt;footer&gt;.</param>
        public Footer(IHtmlComponent component)
        {
            Children = new List<IHtmlComponent>() { component };
        }

        /// <summary>
        /// Initiate a new &lt;footer&gt; with text.
        /// </summary>
        /// <param name="text">The text for the content of the &lt;footer&gt;.</param>
        public Footer(string text)
        {
            Children = new List<IHtmlComponent>() { new Text(text) };
        }

        /// <summary>
        /// Initiate a new footer with the given content. Lets elements be nested by
        /// passing their children as arguments.
        /// </summary>
        /// <param name="content">The elements or components for the content of the footer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Footer(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

    }
}
