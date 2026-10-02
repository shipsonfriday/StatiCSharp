using StatiCSharp.Interfaces;
using System.Globalization; // CultureInfo

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
    public IReadOnlyList<string> MakeSectionsFor { get; private set; } = [];

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
    /// <param name="language">An IETF language tag, e.g. "en-US", "de-DE" or "de".</param>
    /// <returns>this - the website itself.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="language"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="language"/> is empty, only whitespace, or not a language tag.</exception>
    /// <exception cref="CultureNotFoundException"><paramref name="language"/> is not a language this machine knows.</exception>
    public Website WithLanguage(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        string tag = language.Trim();
        RejectAnUnderscore(tag, nameof(language));

        // predefinedOnly, because the plain constructor invents a culture for anything that
        // merely looks like a tag: new CultureInfo("klingon") succeeds and formats in English.
        return WithLanguage(CultureInfo.GetCultureInfo(tag, predefinedOnly: true));
    }

    /// <summary>
    /// Sets the language the website's main content is written in.
    /// Defaults to "en-US" when not set.
    /// </summary>
    /// <param name="language">The culture of the main content.</param>
    /// <returns>this - the website itself.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="language"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="language"/> is the invariant culture, or its name is not a language tag.</exception>
    /// <exception cref="CultureNotFoundException"><paramref name="language"/> is not a language this machine knows.</exception>
    public Website WithLanguage(CultureInfo language)
    {
        ArgumentNullException.ThrowIfNull(language);

        // Checked here as well as in the overload above, because the culture's name is written
        // into the lang attribute of every page. A culture the runtime made up on the spot
        // produces an invalid one and formats dates in English without saying so.
        if (language.Name.Length == 0)
        {
            throw new ArgumentException(
                "The invariant culture cannot be a website's language: it would leave the lang "
                + "attribute of every page empty. Name the language, e.g. \"en-US\" or \"de\".",
                nameof(language));
        }

        RejectAnUnderscore(language.Name, nameof(language));
        CultureInfo.GetCultureInfo(language.Name, predefinedOnly: true);

        Language = language;
        return this;
    }

    /// <summary>
    /// Rejects a culture name with an underscore in it.
    /// <para>
    /// .NET reads <c>en_US</c> as English with a US sort order and accepts it, so it reaches the
    /// lang attribute as <c>lang="en_us"</c> - which is not a valid language tag. Nobody naming
    /// a website's language means a sort order; they mean a hyphen.
    /// </para>
    /// </summary>
    private static void RejectAnUnderscore(string tag, string parameterName)
    {
        if (!tag.Contains('_', StringComparison.Ordinal))
        {
            return;
        }

        throw new ArgumentException(
            $"\"{tag}\" is not a language tag. The parts of a tag are separated by hyphens, so "
            + $"this is probably meant to be \"{tag.Replace('_', '-')}\". An underscore selects an "
            + "alternate sort order in .NET, which is not a language.",
            parameterName);
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
