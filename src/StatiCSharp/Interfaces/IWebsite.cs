using System.Collections.Generic;
using System.Globalization; // CultureInfo

namespace StatiCSharp.Interfaces
{
    /// <summary>
    /// Interface to confrom to for a website object while building templates.
    /// </summary>
    public interface IWebsite
    {
        /// <summary>
        /// The absolute domain of the website. E.g. "https://mydomain.com".
        /// </summary>
        string Url { get; }

        /// <summary>
        /// The name of the website.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A short description of the website. Is used for metadata in the html sites.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The language the websites main content is written in.
        /// </summary>
        CultureInfo Language { get; }

        /// <summary>
        /// Represents the index (homepage) of the website.
        /// </summary>
        IIndex Index { get; }

        /// <summary>
        /// The collection of pages the website contains.
        /// </summary>
        List<IPage> Pages { get; }

        /// <summary>
        /// The collection of sections the website's sections (not pages).
        /// </summary>
        List<ISection> Sections { get; }

        /// <summary>
        /// Collection of the websites section-names. Folders in the content directory with names matching one item of this list a treated as sections.
        /// </summary>
        List<string> MakeSectionsFor { get; }

    }
}
