namespace StatiCSharp.Tools;

/// <summary>
/// The last segment of a site's url: what the author asked for, or the filename.
/// </summary>
internal static class SiteSegment
{
    /// <summary>
    /// The <c>path</c> from the front matter when there is one, otherwise the filename without
    /// its extension.
    /// <para>
    /// A <c>path</c> that keeps nothing usable in a url - <c>...</c>, say, or <c>../..</c> -
    /// falls back to the filename as well. Without that the site would have no segment of its
    /// own and be written over the page in the directory above it.
    /// </para>
    /// </summary>
    /// <param name="path">The <c>path</c> entry of the front matter, empty when not given.</param>
    /// <param name="markdownFileName">The name of the markdown file the site was read from.</param>
    /// <exception cref="ArgumentNullException">An argument is null.</exception>
    internal static string Of(string path, string markdownFileName)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(markdownFileName);

        return path.Length > 0 && UrlPath.HasASegment(path)
            ? path
            : FilenameToPath.From(markdownFileName);
    }
}
