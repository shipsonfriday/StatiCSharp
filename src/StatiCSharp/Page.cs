using StatiCSharp.Interfaces;
using StatiCSharp.Tools;

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
    /// The relative url of the page: the folders it lies in, then the <c>path</c> from the
    /// front matter or, without one, the filename. The renderer writes the page where this
    /// says.
    /// <para>
    /// An "index" segment is dropped, because "about/index.md" is written to "/about" and not
    /// to "/about/index".
    /// </para>
    /// </summary>
    public string Url
    {
        get
        {
            string segment = SiteSegment.Of(Path, MarkdownFileName);

            return UrlPath.From(Hierarchy, segment == "index" ? string.Empty : segment);
        }
    }

    public string MarkdownFileName { get; set; } = string.Empty;

    public string MarkdownFilePath { get; set; } = string.Empty;

    public IReadOnlyList<string> Tags { get; set; } = [];

    public string Content { get; set; } = string.Empty;
}
