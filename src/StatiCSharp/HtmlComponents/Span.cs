using StatiCSharp.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace StatiCSharp.HtmlComponents
{
    /// <summary>
    /// A representation of a &lt;span&gt;&lt;/span&gt; element.
    /// Call the Render() method to turn it into an HTML string.
    /// </summary>
    public class Span : HtmlElement, IHtmlComponent
    {
        private protected override string TagName
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
            Content = new List<IHtmlComponent>() { component };
        }

        /// <summary>
        /// Initiate a new &lt;span&gt; element with text.
        /// </summary>
        /// <param name="text">The text for the content of the span.</param>
        public Span(string text)
        {
            Content = new List<IHtmlComponent>() { new Text(text) };
        }

    }
}
