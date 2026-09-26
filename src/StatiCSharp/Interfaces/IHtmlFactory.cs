using System.Collections.Generic;

namespace StatiCSharp.Interfaces
{
    /// <summary>
    /// Interface to implement for making StatiC# compatible custom themes.
    /// </summary>
    public interface IHtmlFactory
    {
        /// <summary>
        /// Path to the corresponding resources. All files within this directory will be copied to the root directory of the output.
        /// </summary>
        string ResourcesPath { get; }

        /// <summary>
        /// Creates html-code for inside the &lt;head&gt;&lt;/head&gt;-tag. This code is added to all sites.
        /// </summary>
        /// <returns>A string containing the html-code.</returns>
        string MakeHeadHtml();

        /// <summary>
        /// Method that returns the html-code for the index site.
        /// </summary>
        /// <param name="index">The index page to render.</param>
        /// <returns>A string containing the html-code.</returns>
        string MakeIndexHtml(IIndex index);

        /// <summary>
        /// Method that returns the html-code for a page (not section or item).
        /// </summary>
        /// <param name="page">The page to render.</param>
        /// <returns>A string containing the html-code.</returns>
        string MakePageHtml(IPage page);

        /// <summary>
        /// Method that returns the html-code for a section site.
        /// </summary>
        /// <param name="section">The section to render.</param>
        /// <returns>A string containing the html-code.</returns>
        string MakeSectionHtml(ISection section);

        /// <summary>
        /// Method that returns the html-code for an item site.
        /// </summary>
        /// <param name="item">The item to render.</param>
        /// <returns>A string containing the html-code.</returns>
        string MakeItemHtml(IItem item);

        /// <summary>
        /// Method that returns the html-code for the taglist site.
        /// </summary>
        /// <param name="items">A list of the items that include this tag.</param>
        /// <param name="tag">The name of the tag.</param>
        /// <returns>A string containing the html-code.</returns>
        string MakeTagListHtml(List<IItem> items, string tag);
    }
}
