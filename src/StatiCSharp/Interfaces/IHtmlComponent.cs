namespace StatiCSharp.Interfaces
{
    /// <summary>
    /// Interface that all html-components must implement to ensure the output/render as a string containing html-code.
    /// </summary>
    public interface IHtmlComponent
    {
        /// <summary>
        /// Renders the html component as a string.
        /// </summary>
        /// <returns>A string containing the components html-code.</returns>
        string Render();
    }
}
