using StatiCSharp.Interfaces;
using System.Text;

namespace StatiCSharp.HtmlComponents
{
    /// <summary>
    /// A representation of a &lt;p&gt;&lt;/p&gt; element.
    /// Call the Render() method to turn it into an HTML string.
    /// </summary>
    public class Paragraph : HtmlElement<Paragraph>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName
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
            Children = new List<IHtmlComponent>() { component };
        }

        /// <summary>
        /// Initiate a new &lt;p&gt; element with text.
        /// </summary>
        /// <param name="text">The text for the content of the paragraph.</param>
        public Paragraph(string text)
        {
            Children = new List<IHtmlComponent>() { new Text(text) };
        }

        /// <summary>
        /// Initiate a new paragraph with the given content. Lets elements be nested by
        /// passing their children as arguments.
        /// </summary>
        /// <param name="content">The elements or components for the content of the paragraph.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Paragraph(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

    }
}
