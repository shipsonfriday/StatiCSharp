using StatiCSharp.Interfaces;
using StatiCSharp.Tools;

namespace StatiCSharp;

internal class Item : IItem
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    public DateOnly DateLastModified { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// The relative url of the item: its section, then the <c>path</c> from the front matter
    /// or, without one, the filename. The renderer writes the item where this says.
    /// </summary>
    public string Url => UrlPath.From(Section, SiteSegment.Of(Path, MarkdownFileName));

    public string Section { get; set; } = string.Empty;

    public string MarkdownFileName { get; set; } = string.Empty;

    public string MarkdownFilePath { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new List<string>();

    public string Content { get; set; } = string.Empty;
}
