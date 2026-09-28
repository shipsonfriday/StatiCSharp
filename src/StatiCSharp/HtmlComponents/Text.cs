using StatiCSharp.Interfaces;

namespace StatiCSharp.HtmlComponents
{
    /// <summary>
    /// A representation of text.
    /// Call the Render() method to turn it into an HTML string.
    /// </summary>
    public class Text: IHtmlComponent
    {
        // Contains the text as a string
        private string _Text { get; set; }

        /// <summary>
        /// Initiate an empty Text.
        /// </summary>
        public Text()
        {
            _Text = string.Empty;
        }

        /// <summary>
        /// Initiate a new text.
        /// </summary>
        /// <param name="text">Your text.</param>
        public Text(string text)
        {
            _Text = text;
        }
        /// <summary>
        /// Initiate a new text from string.
        /// </summary>
        /// <param name="text">The text as string.</param>
        /// <returns></returns>
        public Text Add(string text)
        {
            _Text = text;
            return this;
        }

        /// <summary>
        /// Renders the text into a html.
        /// </summary>
        /// <returns>The text as an html string.</returns>
        public string Render()
        {
            return _Text;
        }
    }
}
