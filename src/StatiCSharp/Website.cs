using StatiCSharp.Interfaces;
using System.Collections.Generic;
using System.Globalization; // CultureInfo
using System.Linq;

namespace StatiCSharp;

/// <summary>
/// Provides a website as a data object.
/// </summary>
public partial class Website : IWebsite
{
    /// <inheritdoc/>
    public string Url { get; set; }

    /// <inheritdoc/>
    public string Name { get; set; }

    /// <inheritdoc/>
    public string Description { get; set; }

    /// <inheritdoc/>
    public CultureInfo Language { get; set; }

    /// <inheritdoc/>
    public IIndex Index { get; set; }

    /// <inheritdoc/>
    public List<IPage> Pages { get; set; }

    /// <inheritdoc/>
    public List<ISection> Sections { get; set; }

    /// <inheritdoc/>
    public List<string> MakeSectionsFor { get; set; }

    /// <summary>
    /// Initialize a website.
    /// </summary>
    /// <param name="url">The complete domain of the website.</param>
    /// <param name="name">The name of the website.</param>
    /// <param name="description">A short description of the website. Is used for metadata in the html sites.</param>
    /// <param name="language">The language the websites main content is written in.</param>
    /// <param name="sections">Collection of the websites section-names. Folders in the content directory with names matching one item of this list a treated as sections.</param>
    public Website(string url, string name, string description, string language, string sections)
    {
        Url                 = url;
        Name                = name;
        Description         = description;
        Language            = new CultureInfo(language);
        MakeSectionsFor     = sections.Replace(" ", string.Empty).Split(',').ToList();
        Index               = new Index();
        Pages               = new List<IPage>();
        Sections            = new List<ISection>();
    }
}