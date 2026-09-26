using StatiCSharp.Interfaces;
using System;
using System.Collections.Generic;

namespace StatiCSharp.HtmlComponents
{
    /// <summary>
    /// A representation of an &lt;a&gt;&lt;/a&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class A : HtmlElement, IHtmlComponent
    {
        private protected override string TagName
        {
            get { return "a"; }
        }

        /// <summary>
        /// Initiate a new and empty anker-element.
        /// </summary>
        public A()
        {
            // No action needed, because the base class already initialized an empty List<IHtmlComponent>.
        }

        /// <summary>
        /// Initiate a new link with another element or component inside.
        /// </summary>
        /// <param name="component"></param>
        public A(IHtmlComponent component)
        {
            Content = new List<IHtmlComponent>() { component };
        }

        /// <summary>
        /// Initiate a new link with a text-body.
        /// </summary>
        /// <param name="text">The text for the link.</param>
        public A(string text)
        {
            Content = new List<IHtmlComponent>();
            Content.Add(new Text(text));
        }

        /// <summary>
        /// Set the content of the href attribute.
        /// </summary>
        /// <param name="href">The target of the link.</param>
        /// <returns>this - the element itself.</returns>
        public A Href(string href)
        {
            Attributes["href"] = href;
            return this;
        }

        /// <summary>
        /// Where to display the linked URL.<br/><br/>
        /// <para>
        /// The following keywords have special meanings for where to load the URL:<br/>
        /// "_self": the current browsing context. (Default)<br/>
        /// "_blank": usually a new tab, but users can configure browsers to open a new window instead.<br/>
        /// "_parent": the parent browsing context of the current one. If no parent, behaves as "_self".<br/>
        /// "_top": the topmost browsing context (the "highest" context that's an ancestor of the current one).
        /// If no ancestors, behaves as "_self".
        /// </para>
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public A Target(string target)
        {
            string[] allowed = new string[] {"_self", "_blank", "_parent", "_top"};
            var exists = Array.Exists(allowed, element => element == target);
            if (exists)
            {
                Attributes["target"] = target;
                return this;
            }
            return this;
        }

    }
}
