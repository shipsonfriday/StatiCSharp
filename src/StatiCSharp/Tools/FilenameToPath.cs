using System;
using System.IO;

namespace StatiCSharp.Tools
{
    /// <summary>
    /// Provides methods to generate paths from filenames.
    /// </summary>
    internal static class FilenameToPath
    {
        /// <summary>
        /// Generates a url path segment from a given filename: the extension is dropped,
        /// spaces become hyphens and the result is lowercased.
        /// <para>
        /// The lowercasing is invariant on purpose. With the culture-sensitive variant, a
        /// machine set to Turkish turns "I" into "ı" instead of "i", which would produce a
        /// different url for the same file.
        /// </para>
        /// </summary>
        /// <param name="filename">The filename, with or without an extension.</param>
        /// <returns>The path segment, e.g. "my-post" for "My Post.md".</returns>
        /// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
        public static string From(string filename)
        {
            ArgumentNullException.ThrowIfNull(filename);

            return Path.GetFileNameWithoutExtension(filename)
                .Trim()
                .Replace(' ', '-')
                .ToLowerInvariant();
        }
    }
}
