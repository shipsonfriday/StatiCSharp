using System.Globalization; // CultureInfo

namespace StatiCSharp.Interfaces;

/// <summary>
/// The configuration of a website: what the author states about it before anything is
/// read.
/// <para>
/// The content that is read from the markdown files is not part of this. It is handed to a
/// theme through <see cref="RenderContext"/>, which carries both. Keeping them apart is
/// what lets a website be configured once and generated any number of times.
/// </para>
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
    /// Collection of the websites section-names. Folders in the content directory with names matching one item of this list a treated as sections.
    /// </summary>
    List<string> MakeSectionsFor { get; }

}
