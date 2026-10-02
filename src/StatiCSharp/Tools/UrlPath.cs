namespace StatiCSharp.Tools;

/// <summary>
/// Builds the relative url of a site, which is also where the site is written.
/// <para>
/// There used to be two calculations: the <c>Url</c> property of each site and, in another
/// file, the output directory the renderer assembled - kept in step by a comment saying so.
/// They could be bent apart, and a <c>path</c> of <c>../..</c> in the front matter did exactly
/// that: it wrote the file outside the output directory, where the clean up never looks.
/// </para>
/// <para>
/// The url is the single answer now. The renderer splits it back into segments to decide where
/// to write, so whatever a link says is where the file is.
/// </para>
/// </summary>
internal static class UrlPath
{
    /// <summary>
    /// Joins the given parts into an absolute, relative-to-the-site url.
    /// <para>
    /// A part may itself hold separators - a page's hierarchy does, and a <c>path</c> from the
    /// front matter may - so every part is split first and each segment goes through
    /// <see cref="UrlSlug"/>. Segments that keep nothing usable are dropped, which is what
    /// stops <c>..</c> and an absolute path from leaving the output directory.
    /// </para>
    /// </summary>
    /// <param name="parts">The parts, from the outermost inwards. Null and empty are ignored.</param>
    /// <returns>The url, e.g. "/posts/my-post". "/" when nothing usable is left.</returns>
    internal static string From(params string?[] parts)
    {
        ArgumentNullException.ThrowIfNull(parts);

        return "/" + string.Join('/', SegmentsOf(parts));
    }

    /// <summary>
    /// The segments of a url built by <see cref="From"/>, which are the directory names the
    /// site is written to below the output directory.
    /// </summary>
    /// <param name="url">The url.</param>
    /// <exception cref="ArgumentNullException"><paramref name="url"/> is null.</exception>
    internal static string[] SegmentsOf(string url)
    {
        ArgumentNullException.ThrowIfNull(url);

        return url.Split('/', StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>
    /// Whether the part keeps at least one segment. A part made up entirely of symbols, or of
    /// nothing but separators and dots, keeps none - and a site with no segment of its own
    /// would be written over the page in the directory above it.
    /// </summary>
    /// <param name="part">The part to check.</param>
    /// <exception cref="ArgumentNullException"><paramref name="part"/> is null.</exception>
    internal static bool HasASegment(string part)
    {
        ArgumentNullException.ThrowIfNull(part);

        return SegmentsOf([part]).Any();
    }

    private static IEnumerable<string> SegmentsOf(string?[] parts)
        => parts
            .Where(part => !string.IsNullOrEmpty(part))
            .SelectMany(part => part!.Split('/', '\\'))
            .Select(UrlSlug.From)
            .Where(segment => segment.Length > 0);
}
