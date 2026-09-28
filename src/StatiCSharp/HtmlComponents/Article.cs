using StatiCSharp.Interfaces;
using System.Collections.Generic;

namespace StatiCSharp.HtmlComponents
{
    /// <summary>
    /// A representation of a &lt;article&gt;&lt;/article&gt; element.
    /// Call the Render() method to turn it into an HTML string.
    /// </summary>
    public class Article : HtmlElement, IHtmlComponent
    {
        private protected override string TagName
        {
            get { return "article"; }
        }

        /// <summary>
        /// Initiate a new empty &lt;article&gt; element.
        /// </summary>
        public Article()
        {
            // No action needed, because the base class already initialized an empty List<IHtmlComponent>.
        }

        /// <summary>
        /// Initiate a new &lt;article&gt; with another element or component inside.
        /// </summary>
        /// <param name="component">The element or component for the content of the &lt;article&gt;.</param>
        public Article(IHtmlComponent component)
        {
            Content = new List<IHtmlComponent>() { component };
        }

        /// <summary>
        /// Initiate a new &lt;article&gt; element with text.
        /// </summary>
        /// <param name="text">The text for the content of the &lt;article&gt;.</param>
        public Article(string text)
        {
            Content = new List<IHtmlComponent>() { new Text(text) };
        }

    }
}
