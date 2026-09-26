using StatiCSharp.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace StatiCSharp.HtmlComponents
{
    /// <summary>
    /// A representation of a HTML element.
    /// Call the Render() method to turn it into an HTML string.
    /// </summary>
    public class Body : HtmlElement, IHtmlComponent
    {
        private protected override string TagName
        {
            get { return "body"; }
        }

        /// <summary>
        /// Initiate a new empty html element.
        /// </summary>
        public Body()
        {
            // No action needed, because the base class already initialized an empty List<IHtmlComponent>.
        }

        /// <summary>
        /// Initiate a new html element with another element or component inside.
        /// </summary>
        /// <param name="element">The element or component for the content of the html tag.</param>
        public Body(IHtmlComponent element)
        {
            Content = new List<IHtmlComponent>();
            Content.Add(element);
        }

        /// <summary>
        /// Initiate a new html element with text.
        /// </summary>
        /// <param name="text">The text for the content of the html tag.</param>
        public Body(string text)
        {
            Content = new List<IHtmlComponent>();
            Content.Add(new Text(text));
        }

    }
}
