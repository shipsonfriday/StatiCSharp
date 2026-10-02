using System.Text;

namespace StatiCSharp.Tools;

/// <summary>
/// Turns text into a segment that works both in a url and as a directory name.
/// <para>
/// Use this when a theme builds a link whose target StatiC# creates, for example a tag
/// page at <c>/tag/{slug}</c>. The generator writes the directory under the same slug,
/// so a link built any other way points at a directory that is not there.
/// </para>
/// </summary>
public static class UrlSlug
{
    /// <summary>
    /// Builds a path segment from the given text: letters and digits are kept and
    /// lowercased, dots, underscores and tildes are kept, and everything else -
    /// whitespace, punctuation, hyphens, path and url separators - becomes a single
    /// hyphen, never two in a row.
    /// <para>
    /// Letters of any script are preserved, so a tag written in Cyrillic or Greek keeps
    /// its own characters instead of collapsing to nothing.
    /// </para>
    /// <para>
    /// The result can be empty, for a text made up entirely of symbols. Callers have to
    /// handle that, because an empty segment would collide with the parent directory.
    /// </para>
    /// </summary>
    /// <param name="text">The text to convert, e.g. a tag name.</param>
    /// <returns>The path segment, e.g. "web-dev" for "Web Dev".</returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> is null.</exception>
    public static string From(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        StringBuilder slug = new(text.Length);

        foreach (char character in text.Trim())
        {
            if (char.IsLetterOrDigit(character))
            {
                slug.Append(char.ToLowerInvariant(character));
            }
            else if (character is '.' or '_' or '~')
            {
                slug.Append(character);
            }
            else if (slug.Length > 0 && slug[^1] != '-')
            {
                // Anything else becomes a separator, but never two in a row. A literal
                // hyphen goes through here as well, so "a -- b" collapses to "a-b"
                // instead of keeping every separator it was written with.
                slug.Append('-');
            }
        }

        // A separator or dot at either end is noise, and a trailing dot is not even a
        // valid directory name on Windows.
        return slug.ToString().Trim('-', '.');
    }
}
