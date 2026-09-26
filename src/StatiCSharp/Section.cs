using StatiCSharp.Interfaces;
using System;
using System.Collections.Generic;

namespace StatiCSharp;

internal class Section : ISection
{
    public string SectionName { get; set; } = string.Empty;

    public List<IItem> Items { get; } = [];

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    public DateOnly DateLastModified { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// The relative url of the section, which is its name.
    /// WebsiteManager.MakeSections writes it to the same place.
    /// </summary>
    public string Url => $"/{SectionName}";

    public string Hierarchy { get; set; } = string.Empty;

    public string MarkdownFileName { get; set; } = string.Empty;

    public string MarkdownFilePath { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new List<string>();

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
        int successor = Items.FindIndex(existing => existing.Date < item.Date);

        if (successor < 0)
        {
            Items.Add(item);
        }
        else
        {
            Items.Insert(successor, item);
        }
    }
}
