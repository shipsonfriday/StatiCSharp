using StatiCSharp.Interfaces;
using System;
using System.Collections.Generic;

namespace StatiCSharp.HtmlComponents
{
    /// <summary>
    /// A representation of a &lt;h1&gt;&lt;/h1&gt; element.
    /// Call the Render() method to turn it into an HTML string.
    /// </summary>
    public class H1 : HtmlElement<H1>, IHtmlComponent
    {
        private protected override string TagName
        {
            get { return "h1"; }
        }

        /// <summary>
        /// Initiate a new empty element.
        /// </summary>
        public H1()
        {
            // No action needed, because the base class already initialized an empty List<IHtmlComponent>.
        }

        /// <summary>
        /// Initiate a new element with another element or component inside.
        /// </summary>
        /// <param name="component">The element or component for the content of the element.</param>
        public H1(IHtmlComponent component)
        {
            Content = new List<IHtmlComponent>() { component };
        }

        /// <summary>
        /// Initiate a new element with text.
        /// </summary>
        /// <param name="text">The text for the content of the element.</param>
        public H1(string text)
        {
            Content = new List<IHtmlComponent>() { new Text(text) };
        }

        /// <summary>
        /// Initiate a new h1 element with the given content. Lets elements be nested by
        /// passing their children as arguments.
        /// </summary>
        /// <param name="content">The elements or components for the content of the h1 element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public H1(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Content = [.. content];
        }
    }


    /// <summary>
    /// A representation of a &lt;h2&gt;&lt;/h2&gt; element.
    /// Call the Render() method to turn it into an HTML string.
    /// </summary>
    public class H2 : HtmlElement<H2>, IHtmlComponent
    {
        private protected override string TagName
        {
            get { return "h2"; }
        }

        /// <summary>
        /// Initiate a new empty element.
        /// </summary>
        public H2()
        {
            // No action needed, because the base class already initialized an empty List<IHtmlComponent>.
        }

        /// <summary>
        /// Initiate a new element with another element or component inside.
        /// </summary>
        /// <param name="component">The element or component for the content of the element.</param>
        public H2(IHtmlComponent component)
        {
            Content = new List<IHtmlComponent>() { component };
        }

        /// <summary>
        /// Initiate a new element with text.
        /// </summary>
        /// <param name="text">The text for the content of the element.</param>
        public H2(string text)
        {
            Content = new List<IHtmlComponent>() { new Text(text) };
        }

        /// <summary>
        /// Initiate a new h2 element with the given content. Lets elements be nested by
        /// passing their children as arguments.
        /// </summary>
        /// <param name="content">The elements or components for the content of the h2 element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public H2(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Content = [.. content];
        }
    }


    /// <summary>
    /// A representation of a &lt;h3&gt;&lt;/h3&gt; element.
    /// Call the Render() method to turn it into an HTML string.
    /// </summary>
    public class H3 : HtmlElement<H3>, IHtmlComponent
    {
        private protected override string TagName
        {
            get { return "h3"; }
        }

        /// <summary>
        /// Initiate a new empty element.
        /// </summary>
        public H3()
        {
            // No action needed, because the base class already initialized an empty List<IHtmlComponent>.
        }

        /// <summary>
        /// Initiate a new element with another element or component inside.
        /// </summary>
        /// <param name="component">The element or component for the content of the element.</param>
        public H3(IHtmlComponent component)
        {
            Content = new List<IHtmlComponent>() { component };
        }

        /// <summary>
        /// Initiate a new element with text.
        /// </summary>
        /// <param name="text">The text for the content of the element.</param>
        public H3(string text)
        {
            Content = new List<IHtmlComponent>() { new Text(text) };
        }

        /// <summary>
        /// Initiate a new h3 element with the given content. Lets elements be nested by
        /// passing their children as arguments.
        /// </summary>
        /// <param name="content">The elements or components for the content of the h3 element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public H3(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Content = [.. content];
        }
    }


    /// <summary>
    /// A representation of a &lt;h4&gt;&lt;/h4&gt; element.
    /// Call the Render() method to turn it into an HTML string.
    /// </summary>
    public class H4 : HtmlElement<H4>, IHtmlComponent
    {
        private protected override string TagName
        {
            get { return "h4"; }
        }

        /// <summary>
        /// Initiate a new empty element.
        /// </summary>
        public H4()
        {
            // No action needed, because the base class already initialized an empty List<IHtmlComponent>.
        }

        /// <summary>
        /// Initiate a new element with another element or component inside.
        /// </summary>
        /// <param name="component">The element or component for the content of the element.</param>
        public H4(IHtmlComponent component)
        {
            Content = new List<IHtmlComponent>() { component };
        }

        /// <summary>
        /// Initiate a new element with text.
        /// </summary>
        /// <param name="text">The text for the content of the element.</param>
        public H4(string text)
        {
            Content = new List<IHtmlComponent>() { new Text(text) };
        }

        /// <summary>
        /// Initiate a new h4 element with the given content. Lets elements be nested by
        /// passing their children as arguments.
        /// </summary>
        /// <param name="content">The elements or components for the content of the h4 element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public H4(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Content = [.. content];
        }
    }


    /// <summary>
    /// A representation of a &lt;h5&gt;&lt;/h5&gt; element.
    /// Call the Render() method to turn it into an HTML string.
    /// </summary>
    public class H5 : HtmlElement<H5>, IHtmlComponent
    {
        private protected override string TagName
        {
            get { return "h5"; }
        }

        /// <summary>
        /// Initiate a new empty element.
        /// </summary>
        public H5()
        {
            // No action needed, because the base class already initialized an empty List<IHtmlComponent>.
        }

        /// <summary>
        /// Initiate a new element with another element or component inside.
        /// </summary>
        /// <param name="component">The element or component for the content of the element.</param>
        public H5(IHtmlComponent component)
        {
            Content = new List<IHtmlComponent>() { component };
        }

        /// <summary>
        /// Initiate a new element with text.
        /// </summary>
        /// <param name="text">The text for the content of the element.</param>
        public H5(string text)
        {
            Content = new List<IHtmlComponent>() { new Text(text) };
        }

        /// <summary>
        /// Initiate a new h5 element with the given content. Lets elements be nested by
        /// passing their children as arguments.
        /// </summary>
        /// <param name="content">The elements or components for the content of the h5 element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public H5(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Content = [.. content];
        }
    }


    /// <summary>
    /// A representation of a &lt;h6&gt;&lt;/h6&gt; element.
    /// Call the Render() method to turn it into an HTML string.
    /// </summary>
    public class H6 : HtmlElement<H6>, IHtmlComponent
    {
        private protected override string TagName
        {
            get { return "h6"; }
        }

        /// <summary>
        /// Initiate a new empty element.
        /// </summary>
        public H6()
        {
            // No action needed, because the base class already initialized an empty List<IHtmlComponent>.
        }

        /// <summary>
        /// Initiate a new element with another element or component inside.
        /// </summary>
        /// <param name="component">The element or component for the content of the element.</param>
        public H6(IHtmlComponent component)
        {
            Content = new List<IHtmlComponent>() { component };
        }

        /// <summary>
        /// Initiate a new element with text.
        /// </summary>
        /// <param name="text">The text for the content of the element.</param>
        public H6(string text)
        {
            Content = new List<IHtmlComponent>() { new Text(text) };
        }

        /// <summary>
        /// Initiate a new h6 element with the given content. Lets elements be nested by
        /// passing their children as arguments.
        /// </summary>
        /// <param name="content">The elements or components for the content of the h6 element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
        public H6(params IHtmlComponent[] content)
        {
            ArgumentNullException.ThrowIfNull(content);

            Content = [.. content];
        }
    }
}
