using StatiCSharp.Interfaces;
using System;
using System.Collections.Generic;

namespace StatiCSharp.HtmlComponents
{
    /// <summary>
    /// A representation of a &lt;header&gt;&lt;/header&gt; element.
    /// Call the Render() method to turn it into an HTML string.
    /// </summary>
    public class Header: HtmlElement, IHtmlComponent
    {
        private protected override string TagName
        {
            get { return "header"; }
        }
        
        /// <summary>
        /// Inititate a new empty header.
        /// </summary>
        public Header()
        {
            // No action needed, because the base class already initialized an empty List<IHtmlComponent>.
        }

        /// <summary>
        /// Initiate a new header with another element or component inside.
        /// </summary>
        /// <param name="element">The element or component for the content of the header.</param>
        public Header(IHtmlComponent element)
        {
            Content = new List<IHtmlComponent>();
            Content.Add(element);
        }

        /// <summary>
        /// Initiate a new header with text.
        /// </summary>
        /// <param name="text">The text for the content of the header.</param>
        public Header(string text)
        {
            Content= new List<IHtmlComponent>();
            Content.Add(new Text(text));
        }

        /// <summary>
        /// Initiate a new header with the given content. Lets elements be nested by
        /// passing their children as arguments.
        /// </summary>
        /// <param name="content">The elements or components for the content of the header.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Header(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Content = [.. content];
        }

    }
}
