using StatiCSharp.Interfaces;

namespace StatiCSharp.HtmlComponents
{
    /// <summary>
    /// A representation of a &lt;dl&gt;&lt;/dl&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Dl : HtmlElement<Dl>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "dl";

        /// <summary>Initiate a new and empty dl element.</summary>
        public Dl() { }

        /// <summary>Initiate a new dl element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Dl(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new dl element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Dl(string text)
        {
            Children = [new Text(text)];
        }
    }

    /// <summary>
    /// A representation of a &lt;dt&gt;&lt;/dt&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Dt : HtmlElement<Dt>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "dt";

        /// <summary>Initiate a new and empty dt element.</summary>
        public Dt() { }

        /// <summary>Initiate a new dt element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Dt(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new dt element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Dt(string text)
        {
            Children = [new Text(text)];
        }
    }

    /// <summary>
    /// A representation of a &lt;dd&gt;&lt;/dd&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Dd : HtmlElement<Dd>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "dd";

        /// <summary>Initiate a new and empty dd element.</summary>
        public Dd() { }

        /// <summary>Initiate a new dd element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Dd(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new dd element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Dd(string text)
        {
            Children = [new Text(text)];
        }
    }
}
