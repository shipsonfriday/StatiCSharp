using StatiCSharp.Interfaces;
using System;

namespace StatiCSharp.HtmlComponents
{
    /// <summary>
    /// A representation of a &lt;strong&gt;&lt;/strong&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Strong : HtmlElement<Strong>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "strong";

        /// <summary>Initiate a new and empty strong element.</summary>
        public Strong() { }

        /// <summary>Initiate a new strong element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Strong(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new strong element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Strong(string text)
        {
            Children = [new Text(text)];
        }
    }

    /// <summary>
    /// A representation of a &lt;em&gt;&lt;/em&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Em : HtmlElement<Em>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "em";

        /// <summary>Initiate a new and empty em element.</summary>
        public Em() { }

        /// <summary>Initiate a new em element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Em(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new em element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Em(string text)
        {
            Children = [new Text(text)];
        }
    }

    /// <summary>
    /// A representation of a &lt;small&gt;&lt;/small&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Small : HtmlElement<Small>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "small";

        /// <summary>Initiate a new and empty small element.</summary>
        public Small() { }

        /// <summary>Initiate a new small element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Small(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new small element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Small(string text)
        {
            Children = [new Text(text)];
        }
    }

    /// <summary>
    /// A representation of a &lt;mark&gt;&lt;/mark&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Mark : HtmlElement<Mark>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "mark";

        /// <summary>Initiate a new and empty mark element.</summary>
        public Mark() { }

        /// <summary>Initiate a new mark element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Mark(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new mark element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Mark(string text)
        {
            Children = [new Text(text)];
        }
    }

    /// <summary>
    /// A representation of a &lt;code&gt;&lt;/code&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Code : HtmlElement<Code>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "code";

        /// <summary>Initiate a new and empty code element.</summary>
        public Code() { }

        /// <summary>Initiate a new code element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Code(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new code element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Code(string text)
        {
            Children = [new Text(text)];
        }
    }

    /// <summary>
    /// A representation of a &lt;pre&gt;&lt;/pre&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Pre : HtmlElement<Pre>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "pre";

        /// <summary>Initiate a new and empty pre element.</summary>
        public Pre() { }

        /// <summary>Initiate a new pre element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Pre(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new pre element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Pre(string text)
        {
            Children = [new Text(text)];
        }
    }

    /// <summary>
    /// A representation of a &lt;blockquote&gt;&lt;/blockquote&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Blockquote : HtmlElement<Blockquote>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "blockquote";

        /// <summary>Initiate a new and empty blockquote element.</summary>
        public Blockquote() { }

        /// <summary>Initiate a new blockquote element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Blockquote(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new blockquote element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Blockquote(string text)
        {
            Children = [new Text(text)];
        }

        /// <summary>Sets the <c>cite</c> attribute.</summary>
        /// <param name="value">The value of the attribute.</param>
        /// <returns>this - the element itself.</returns>
        public Blockquote Cite(string value) => Attribute("cite", value);
    }

    /// <summary>
    /// A representation of a &lt;cite&gt;&lt;/cite&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Cite : HtmlElement<Cite>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "cite";

        /// <summary>Initiate a new and empty cite element.</summary>
        public Cite() { }

        /// <summary>Initiate a new cite element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Cite(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new cite element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Cite(string text)
        {
            Children = [new Text(text)];
        }
    }

    /// <summary>
    /// A representation of a &lt;abbr&gt;&lt;/abbr&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Abbr : HtmlElement<Abbr>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "abbr";

        /// <summary>Initiate a new and empty abbr element.</summary>
        public Abbr() { }

        /// <summary>Initiate a new abbr element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Abbr(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new abbr element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Abbr(string text)
        {
            Children = [new Text(text)];
        }
    }

    /// <summary>
    /// A representation of a &lt;sub&gt;&lt;/sub&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Sub : HtmlElement<Sub>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "sub";

        /// <summary>Initiate a new and empty sub element.</summary>
        public Sub() { }

        /// <summary>Initiate a new sub element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Sub(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new sub element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Sub(string text)
        {
            Children = [new Text(text)];
        }
    }

    /// <summary>
    /// A representation of a &lt;sup&gt;&lt;/sup&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Sup : HtmlElement<Sup>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "sup";

        /// <summary>Initiate a new and empty sup element.</summary>
        public Sup() { }

        /// <summary>Initiate a new sup element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Sup(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new sup element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Sup(string text)
        {
            Children = [new Text(text)];
        }
    }

    /// <summary>
    /// A representation of a &lt;time&gt;&lt;/time&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Time : HtmlElement<Time>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "time";

        /// <summary>Initiate a new and empty time element.</summary>
        public Time() { }

        /// <summary>Initiate a new time element with the given content.</summary>
        /// <param name="content">The elements or components inside the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public Time(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Children = [.. content];
        }

        /// <summary>Initiate a new time element with the given text.</summary>
        /// <param name="text">The text inside the element.</param>
        public Time(string text)
        {
            Children = [new Text(text)];
        }

        /// <summary>Sets the <c>datetime</c> attribute.</summary>
        /// <param name="value">The value of the attribute.</param>
        /// <returns>this - the element itself.</returns>
        public Time DateTime(string value) => Attribute("datetime", value);
    }
}
