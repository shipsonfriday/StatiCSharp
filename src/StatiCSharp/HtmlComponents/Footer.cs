using StatiCSharp.Interfaces;
using System.Collections.Generic;

namespace StatiCSharp.HtmlComponents
{
    /// <summary>
    /// A representation of a &lt;footer&gt;&lt;/footer&gt; element.
    /// Call the Render() method to turn it into an HTML string.
    /// </summary>
    public class Footer : HtmlElement, IHtmlComponent
    {
        private protected override string TagName
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
            Content = new List<IHtmlComponent>() { component };
        }

        /// <summary>
        /// Initiate a new &lt;footer&gt; with text.
        /// </summary>
        /// <param name="text">The text for the content of the &lt;footer&gt;.</param>
        public Footer(string text)
        {
            Content = new List<IHtmlComponent>() { new Text(text) };
        }

    }
}
