using System;
using System.Collections.Generic;

namespace StatiCSharp.Interfaces
{
    /// <summary>
    /// The interface that all sites (index, pages, sections, items) must implement.<br/>
    /// Some sites may add additional interfaces that inherit from this one.
    /// </summary>
    public interface ISite
    {
        /// <summary>
        /// The title of the site. E.g used for the &lt;title&gt;-tag in the browser.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// A short description of the site. Used e.g. as teaser of the content when listing items.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// The name of the author of the page.
        /// </summary>
        string Author { get; set; }

        /// <summary>
        /// The date when this site is published or should be.
        /// </summary>
        DateOnly Date { get; set; }

        /// <summary>
        /// The date when the markdown-file was last modified.
        /// </summary>
        DateOnly DateLastModified { get; set; }

        /// <summary>
        /// The path of the site within its hierachy, given by the users meta data.
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// The relative url of the item.
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// The tags that are associated with the site.
        /// </summary>
        List<string> Tags { get; set; }

        /// <summary>
        /// The content of the site, given by the associated markdown file.
        /// </summary>
        string Content { get; set; }

        /// <summary>
        ///  Filename of the file from where the site is generated
        /// </summary>
        string MarkdownFileName { get; set; }

        /// <summary>
        /// Path of the markdown-file from where the site in generated
        /// </summary>
        string MarkdownFilePath { get; set; }
    }
}
