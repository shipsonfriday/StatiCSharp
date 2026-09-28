using StatiCSharp.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace StatiCSharp.HtmlComponents
{
    /// <summary>
    /// A representation of a <div></div> element.
    /// Call the Render() method to turn it into an HTML string.
    /// </summary>
    public class Div : HtmlElement, IHtmlComponent
    {
        private protected override string TagName
        {
            get { return "div"; }
        }

        /// <summary>
        /// Initiate a new empty div.
        /// </summary>
        public Div()
        {
            // No action needed, because the base class already initialized an empty List<IHtmlComponent>.
        }

        /// <summary>
        /// Initiate a new div with another element or component inside.
        /// </summary>
        /// <param name="component">The element or component for the content of the div.</param>
        public Div(IHtmlComponent component)
        {
            Content = new List<IHtmlComponent>() { component };
        }

        /// <summary>
        /// Initiate a new div with text.
        /// </summary>
        /// <param name="text">The text for the content of the div.</param>
        public Div(string text)
        {
            Content = new List<IHtmlComponent>() { new Text(text) };
        }

    }
}
