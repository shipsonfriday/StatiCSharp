using StatiCSharp.Interfaces;

namespace StatiCSharp.HtmlComponents
{
    /// <summary>
    /// A representation of a &lt;img /&gt; element.
    /// Call the Render() method to turn it into an HTML string.
    /// </summary>
    public class Image : HtmlElement, IHtmlComponent
    {
        private protected override string TagName
        {
            get { return "img"; }
        }

        private protected override bool VoidElement
        {
            get { return true; }
        }

        /// <summary>
        /// Initiate a new empty image.
        /// </summary>
        public Image()
        {
            // No action needed, because the base class already initialized an empty List<IHtmlComponent>.
            // But because this is a void element the content is ignored through the rendering process.
        }

        /// <summary>
        /// Initiate a new image from a given source.
        /// </summary>
        /// <param name="src">The source path of the image.</param>
        public Image(string src)
        {
            Attributes["src"] = src;
        }

        /// <summary>
        /// Set the source path of the image.
        /// </summary>
        /// <param name="src">The path to the image.</param>
        /// <returns>this - the element itself.</returns>
        public Image Src(string src)
        {
            Attributes["src"] = src;
            return this;
        }

        /// <summary>
        /// Add an alternate text.
        /// </summary>
        /// <param name="alt">The alternative text.</param>
        /// <returns>this - the element itself.</returns>
        public Image Alt(string alt)
        {
            Attributes["alt"] = alt;
            return this;
        }
    }
}
