using StatiCSharp.Interfaces;
using System.Text;

namespace StatiCSharp.HtmlComponents
{
    /// <summary>
    /// A representation of a HTML element.
    /// Call the Render() method to turn it into an HTML string.
    /// </summary>
    public class Body : HtmlElement<Body>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName
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
            Children = new List<IHtmlComponent>();
            Children.Add(element);
        }

        /// <summary>
        /// Initiate a new html element with text.
        /// </summary>
        /// <param name="text">The text for the content of the html tag.</param>
        public Body(string text)
        {
            Children = new List<IHtmlComponent>();
            Children.Add(new Text(text));
        }

        /// <summary>
        /// Initiate a new body with the given content. Lets elements be nested by
        /// passing their children as arguments.
        /// </summary>
        /// <param name="content">The elements or components for the content of the body.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Body(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

    }
}
