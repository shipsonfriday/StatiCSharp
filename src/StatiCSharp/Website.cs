using StatiCSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization; // CultureInfo
using System.Linq;

namespace StatiCSharp;

/// <summary>
/// Provides a website as a data object.
/// <para>
/// Start with <see cref="Create(string, string)"/> and add the optional parts fluently:
/// <code>
/// var website = Website.Create(url: "https://example.com", name: "My Website")
///     .WithDescription("A short description")
///     .WithLanguage("en-US")
///     .WithSections("posts", "about");
/// </code>
/// </para>
/// </summary>
public class Website : IWebsite
{
    /// <summary>
    /// Used when no language is given. Explicit, so the output never depends on the
    /// culture of the machine the generator happens to run on.
    /// </summary>
    private static readonly CultureInfo DefaultLanguage = new("en-US");

    /// <inheritdoc/>
    public string Url { get; private set; }

    /// <inheritdoc/>
    public string Name { get; private set; }

    /// <inheritdoc/>
    public string Description { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public CultureInfo Language { get; private set; } = DefaultLanguage;

    /// <inheritdoc/>
    public IIndex Index { get; } = new Index();

    /// <inheritdoc/>
    public List<IPage> Pages { get; } = [];

    /// <inheritdoc/>
    public List<ISection> Sections { get; } = [];

    /// <inheritdoc/>
    public List<string> MakeSectionsFor { get; private set; } = [];

    private Website(string url, string name)
    {
        Url = url;
        Name = name;
    }

    /// <summary>
    /// Starts a new website. These two values are required, everything else is optional
    /// and added with the <c>With…</c> methods.
    /// </summary>
    /// <param name="url">The complete domain of the website, e.g. "https://example.com".</param>
    /// <param name="name">The name of the website.</param>
    /// <returns>The new website, ready for further configuration.</returns>
    /// <exception cref="ArgumentNullException">A value is null.</exception>
    /// <exception cref="ArgumentException">A value is empty or only whitespace.</exception>
    public static Website Create(string url, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Website(url.Trim(), name.Trim());
    }

    /// <summary>
    /// Sets a short description of the website. Used for metadata in the html sites.
    /// </summary>
    /// <param name="description">The description.</param>
    /// <returns>this - the website itself.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="description"/> is null.</exception>
    public Website WithDescription(string description)
    {
        ArgumentNullException.ThrowIfNull(description);

        Description = description;
        return this;
    }

    /// <summary>
    /// Sets the language the website's main content is written in.
    /// Defaults to "en-US" when not set.
    /// </summary>
    /// <param name="language">An IETF language tag, e.g. "en-US" or "de-DE".</param>
    /// <returns>this - the website itself.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="language"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="language"/> is empty or only whitespace.</exception>
    /// <exception cref="CultureNotFoundException"><paramref name="language"/> is not a known language tag.</exception>
    public Website WithLanguage(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        return WithLanguage(new CultureInfo(language.Trim()));
    }

    /// <summary>
    /// Sets the language the website's main content is written in.
    /// Defaults to "en-US" when not set.
    /// </summary>
    /// <param name="language">The culture of the main content.</param>
    /// <returns>this - the website itself.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="language"/> is null.</exception>
    public Website WithLanguage(CultureInfo language)
    {
        ArgumentNullException.ThrowIfNull(language);

        Language = language;
        return this;
    }

    /// <summary>
    /// Sets which folders in the content directory are treated as sections.
    /// Replaces any previously set section names.
    /// </summary>
    /// <param name="sections">The section names. Entries are trimmed; empty ones are ignored.</param>
    /// <returns>this - the website itself.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="sections"/> is null.</exception>
    public Website WithSections(params string[] sections)
    {
        ArgumentNullException.ThrowIfNull(sections);

        MakeSectionsFor = sections
            .Where(section => !string.IsNullOrWhiteSpace(section))
            .Select(section => section.Trim())
            .ToList();

        return this;
    }
}
