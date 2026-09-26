namespace StatiCSharp.Tools
{
    /// <summary>
    /// Provides methods to generate paths from filenames.
    /// </summary>
    internal static class FilenameToPath
    {
        /// <summary>
        /// Generates a path from a given filename.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        static public string From(string filename)
        {
            return filename.Substring(0, filename.LastIndexOf(".")).Replace(" ", "-").Trim().ToLower();
        }
    }
}
