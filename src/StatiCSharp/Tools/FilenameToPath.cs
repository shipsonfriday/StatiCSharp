namespace StatiCSharp.Tools
{
    /// <summary>
    /// Provides methods to generate paths from filenames.
    /// </summary>
    internal static class FilenameToPath
    {
        /// <summary>
        /// Generates a url path segment from a given filename: the extension is dropped and
        /// the rest goes through <see cref="UrlSlug"/>, so items and tags follow one scheme.
        /// </summary>
        /// <param name="filename">The filename, with or without an extension.</param>
        /// <returns>The path segment, e.g. "my-post" for "My Post.md".</returns>
        /// <exception cref="ArgumentNullException"><paramref name="filename"/> is null.</exception>
        public static string From(string filename)
        {
            ArgumentNullException.ThrowIfNull(filename);

            return UrlSlug.From(Path.GetFileNameWithoutExtension(filename));
        }
    }
}
