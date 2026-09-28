using StatiCSharp.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace StatiCSharp.HtmlComponents
{
    /// <summary>
    /// A representation of a &lt;p&gt;&lt;/p&gt; element.
    /// Call the Render() method to turn it into an HTML string.
    /// </summary>
    public class Paragraph : HtmlElement, IHtmlComponent
    {
        private protected override string TagName
        {
            get { return "p"; }
        }
        /// <summary>
        /// Initiate a new empty &lt;p&gt; element.
        /// </summary>
        public Paragraph()
        {
            // No action needed, because the base class already initialized an empty List<IHtmlComponent>.
        }

        /// <summary>
        /// Initiate a new &lt;paragraph&gt; element with another element or component inside.
        /// </summary>
        /// <param name="component">The element or component for the content of the paragraph.</param>
        public Paragraph(IHtmlComponent component)
        {
            Content = new List<IHtmlComponent>() { component };
        }

        /// <summary>
        /// Initiate a new &lt;p&gt; element with text.
        /// </summary>
        /// <param name="text">The text for the content of the paragraph.</param>
        public Paragraph(string text)
        {
            Content = new List<IHtmlComponent>() { new Text(text) };
        }

    }
}
