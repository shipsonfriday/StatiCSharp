using StatiCSharp.Interfaces;
using StatiCSharp.Tools;
using System;
using System.Collections.Generic;

namespace StatiCSharp;

/// <summary>
/// Represenation of a page.
/// </summary>
internal class Page : IPage
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    public DateOnly DateLastModified { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Hierarchy of the page, as a relative file system path, e.g. "docs" or "docs/guide".
    /// </summary>
    public string Hierarchy { get; set; } = string.Empty;

    /// <summary>
    /// The relative url of the page.
    /// <para>
    /// Mirrors what WebsiteManager.MakePages writes: the segment comes from
    /// <see cref="FilenameToPath"/>, and an "index" segment is dropped, because
    /// "about/index.md" is written to "/about" and not to "/about/index".
    /// </para>
    /// <para>
    /// <see cref="Hierarchy"/> is a file system path, so its separators are translated
    /// to forward slashes. Without that, a page generated on Windows carries a backslash
    /// into the url.
    /// </para>
    /// </summary>
    public string Url
    {
        get
        {
            string segment = string.IsNullOrEmpty(Path)
                ? FilenameToPath.From(MarkdownFileName)
                : Path;

            if (segment == "index")
            {
                segment = string.Empty;
            }

            string hierarchy = Hierarchy.Replace('\\', '/').Trim('/');

            if (hierarchy.Length == 0)
            {
                return $"/{segment}";
            }

            return segment.Length == 0 ? $"/{hierarchy}" : $"/{hierarchy}/{segment}";
        }
    }

    public string MarkdownFileName { get; set; } = string.Empty;

    public string MarkdownFilePath { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new List<string>();

    public string Content { get; set; } = string.Empty;
}
