using StatiCSharp.Interfaces;

namespace StatiCSharp.HtmlComponents
{
    /// <summary>
    /// A representation of a &lt;picture&gt;&lt;/picture&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Picture : HtmlElement<Picture>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "picture";

        /// <summary>Initiate a new and empty picture element.</summary>
        public Picture() { }

        /// <summary>Initiate a new picture element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Picture(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new picture element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Picture(string text)
        {
            Children = [new Text(text)];
        }
    }

    /// <summary>
    /// A representation of a &lt;source&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Source : HtmlElement<Source>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "source";

        /// <inheritdoc/>
        protected override bool VoidElement => true;

        /// <summary>Initiate a new source element.</summary>
        public Source() {{ }}

        /// <summary>Sets the <c>src</c> attribute.</summary>
        /// <param name="value">The value of the attribute.</param>
        /// <returns>this - the element itself.</returns>
        public Source Src(string value) => Attribute("src", value);

        /// <summary>Sets the <c>srcset</c> attribute.</summary>
        /// <param name="value">The value of the attribute.</param>
        /// <returns>this - the element itself.</returns>
        public Source Srcset(string value) => Attribute("srcset", value);

        /// <summary>Sets the <c>type</c> attribute.</summary>
        /// <param name="value">The value of the attribute.</param>
        /// <returns>this - the element itself.</returns>
        public Source Type(string value) => Attribute("type", value);

        /// <summary>Sets the <c>media</c> attribute.</summary>
        /// <param name="value">The value of the attribute.</param>
        /// <returns>this - the element itself.</returns>
        public Source Media(string value) => Attribute("media", value);
    }

    /// <summary>
    /// A representation of a &lt;video&gt;&lt;/video&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Video : HtmlElement<Video>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "video";

        /// <summary>Initiate a new and empty video element.</summary>
        public Video() { }

        /// <summary>Initiate a new video element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Video(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new video element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Video(string text)
        {
            Children = [new Text(text)];
        }

        /// <summary>Sets the <c>src</c> attribute.</summary>
        /// <param name="value">The value of the attribute.</param>
        /// <returns>this - the element itself.</returns>
        public Video Src(string value) => Attribute("src", value);

        /// <summary>Sets the <c>poster</c> attribute.</summary>
        /// <param name="value">The value of the attribute.</param>
        /// <returns>this - the element itself.</returns>
        public Video Poster(string value) => Attribute("poster", value);

        /// <summary>Sets the <c>controls</c> attribute, which carries no value.</summary>
        /// <returns>this - the element itself.</returns>
        public Video Controls() => Attribute("controls");

        /// <summary>Sets the <c>loop</c> attribute, which carries no value.</summary>
        /// <returns>this - the element itself.</returns>
        public Video Loop() => Attribute("loop");

        /// <summary>Sets the <c>muted</c> attribute, which carries no value.</summary>
        /// <returns>this - the element itself.</returns>
        public Video Muted() => Attribute("muted");
    }

    /// <summary>
    /// A representation of a &lt;audio&gt;&lt;/audio&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Audio : HtmlElement<Audio>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "audio";

        /// <summary>Initiate a new and empty audio element.</summary>
        public Audio() { }

        /// <summary>Initiate a new audio element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Audio(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new audio element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Audio(string text)
        {
            Children = [new Text(text)];
        }

        /// <summary>Sets the <c>src</c> attribute.</summary>
        /// <param name="value">The value of the attribute.</param>
        /// <returns>this - the element itself.</returns>
        public Audio Src(string value) => Attribute("src", value);

        /// <summary>Sets the <c>controls</c> attribute, which carries no value.</summary>
        /// <returns>this - the element itself.</returns>
        public Audio Controls() => Attribute("controls");

        /// <summary>Sets the <c>loop</c> attribute, which carries no value.</summary>
        /// <returns>this - the element itself.</returns>
        public Audio Loop() => Attribute("loop");
    }

    /// <summary>
    /// A representation of a &lt;iframe&gt;&lt;/iframe&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Iframe : HtmlElement<Iframe>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "iframe";

        /// <summary>Initiate a new and empty iframe element.</summary>
        public Iframe() { }

        /// <summary>Initiate a new iframe element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Iframe(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new iframe element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Iframe(string text)
        {
            Children = [new Text(text)];
        }

        /// <summary>Sets the <c>src</c> attribute.</summary>
        /// <param name="value">The value of the attribute.</param>
        /// <returns>this - the element itself.</returns>
        public Iframe Src(string value) => Attribute("src", value);

        /// <summary>Sets the <c>loading</c> attribute.</summary>
        /// <param name="value">The value of the attribute.</param>
        /// <returns>this - the element itself.</returns>
        public Iframe Loading(string value) => Attribute("loading", value);

        /// <summary>Sets the <c>allowfullscreen</c> attribute, which carries no value.</summary>
        /// <returns>this - the element itself.</returns>
        public Iframe Allowfullscreen() => Attribute("allowfullscreen");
    }
}
