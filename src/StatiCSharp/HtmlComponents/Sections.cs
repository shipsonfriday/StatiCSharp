using StatiCSharp.Interfaces;

namespace StatiCSharp.HtmlComponents
{
    /// <summary>
    /// A representation of a &lt;main&gt;&lt;/main&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Main : HtmlElement<Main>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "main";

        /// <summary>Initiate a new and empty main element.</summary>
        public Main() { }

        /// <summary>Initiate a new main element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Main(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new main element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Main(string text)
        {
            Children = [new Text(text)];
        }
    }

    /// <summary>
    /// A representation of a &lt;section&gt;&lt;/section&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Section : HtmlElement<Section>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "section";

        /// <summary>Initiate a new and empty section element.</summary>
        public Section() { }

        /// <summary>Initiate a new section element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Section(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new section element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Section(string text)
        {
            Children = [new Text(text)];
        }
    }

    /// <summary>
    /// A representation of a &lt;aside&gt;&lt;/aside&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Aside : HtmlElement<Aside>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "aside";

        /// <summary>Initiate a new and empty aside element.</summary>
        public Aside() { }

        /// <summary>Initiate a new aside element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Aside(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new aside element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Aside(string text)
        {
            Children = [new Text(text)];
        }
    }

    /// <summary>
    /// A representation of a &lt;figure&gt;&lt;/figure&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Figure : HtmlElement<Figure>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "figure";

        /// <summary>Initiate a new and empty figure element.</summary>
        public Figure() { }

        /// <summary>Initiate a new figure element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Figure(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new figure element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Figure(string text)
        {
            Children = [new Text(text)];
        }
    }

    /// <summary>
    /// A representation of a &lt;figcaption&gt;&lt;/figcaption&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Figcaption : HtmlElement<Figcaption>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "figcaption";

        /// <summary>Initiate a new and empty figcaption element.</summary>
        public Figcaption() { }

        /// <summary>Initiate a new figcaption element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Figcaption(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new figcaption element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Figcaption(string text)
        {
            Children = [new Text(text)];
        }
    }
}
