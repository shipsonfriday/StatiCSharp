using StatiCSharp.Interfaces;
using StatiCSharp.Tools;

namespace StatiCSharp;

internal class Section : ISection
{
    public string SectionName { get; set; } = string.Empty;

    private readonly List<IItem> _items = [];

    public IReadOnlyList<IItem> Items => _items;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    public DateOnly DateLastModified { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// The relative url of the section, which is its folder name as a url segment. The
    /// renderer writes the section where this says, so the two cannot disagree.
    /// </summary>
    public string Url => UrlPath.From(SectionName);

    public string Hierarchy { get; set; } = string.Empty;

    public string MarkdownFileName { get; set; } = string.Empty;

    public string MarkdownFilePath { get; set; } = string.Empty;

    public IReadOnlyList<string> Tags { get; set; } = [];

    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Adds an item and keeps the collection ordered by date, newest first.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/> is null.</exception>
    public void AddItem(IItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        // Insert at the right position instead of re-sorting. List.Sort is unstable, so
        // items sharing a date could swap places between runs, which would make GitMode
        // rewrite unchanged files.
        int successor = _items.FindIndex(existing => existing.Date < item.Date);

        if (successor < 0)
        {
            _items.Add(item);
        }
        else
        {
            _items.Insert(successor, item);
        }
    }
}
