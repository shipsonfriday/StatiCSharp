using System.Collections.Generic;

namespace StatiCSharp.Interfaces
{
    /// <summary>
    /// Interface to implement for making StatiC# compatible custom themes.
    /// <para>
    /// Every method is given the site to render and a <see cref="RenderContext"/> holding the
    /// website's configuration and all of its content. A theme therefore needs no constructor
    /// of its own beyond its own options, and keeps nothing between calls.
    /// </para>
    /// </summary>
    public interface IHtmlFactory
    {
        /// <summary>
        /// Path to the corresponding resources. All files within this directory will be copied to the root directory of the output.
        /// </summary>
        string ResourcesPath { get; }

        /// <summary>
        /// Creates html-code for inside the &lt;head&gt;&lt;/head&gt;-tag.
        /// <para>
        /// Called for every site, with the site in question, so that a theme can add something
        /// that differs per site - a canonical url or open graph tags, for instance. StatiC#
        /// writes the charset, the viewport, the title, the description, the author and the
        /// keywords itself.
        /// </para>
        /// </summary>
        /// <param name="site">The site being rendered.</param>
        /// <param name="context">The website's configuration and content.</param>
        /// <returns>A string containing the html-code.</returns>
        string MakeHeadHtml(ISite site, RenderContext context);

        /// <summary>
        /// Method that returns the html-code for the index site.
        /// </summary>
        /// <param name="index">The index page to render.</param>
        /// <param name="context">The website's configuration and content.</param>
        /// <returns>A string containing the html-code.</returns>
        string MakeIndexHtml(IIndex index, RenderContext context);

        /// <summary>
        /// Method that returns the html-code for a page (not section or item).
        /// </summary>
        /// <param name="page">The page to render.</param>
        /// <param name="context">The website's configuration and content.</param>
        /// <returns>A string containing the html-code.</returns>
        string MakePageHtml(IPage page, RenderContext context);

        /// <summary>
        /// Method that returns the html-code for a section site.
        /// </summary>
        /// <param name="section">The section to render.</param>
        /// <param name="context">The website's configuration and content.</param>
        /// <returns>A string containing the html-code.</returns>
        string MakeSectionHtml(ISection section, RenderContext context);

        /// <summary>
        /// Method that returns the html-code for an item site.
        /// </summary>
        /// <param name="item">The item to render.</param>
        /// <param name="context">The website's configuration and content.</param>
        /// <returns>A string containing the html-code.</returns>
        string MakeItemHtml(IItem item, RenderContext context);

        /// <summary>
        /// Method that returns the html-code for the taglist site.
        /// </summary>
        /// <param name="items">A list of the items that include this tag.</param>
        /// <param name="tag">The name of the tag.</param>
        /// <param name="context">The website's configuration and content.</param>
        /// <returns>A string containing the html-code.</returns>
        string MakeTagListHtml(List<IItem> items, string tag, RenderContext context);
    }
}
