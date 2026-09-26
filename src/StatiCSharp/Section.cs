using StatiCSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StatiCSharp;

internal class Section : ISection
{
    public string SectionName { get; set; } = string.Empty;

    public List<IItem> Items { get; set; } = new List<IItem>();

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    public DateOnly DateLastModified { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    public string Path { get; set; } = string.Empty;

    public string Url
    {
        get
        {
            string x = string.Empty;
            if (Path == string.Empty)
            {
                x = MarkdownFileName.Substring(0, MarkdownFileName.LastIndexOf(".md")).Replace(" ", "-").Trim();
            }
            else
            {
                x = Path;
            }
            return $"/{SectionName}";
        }
    }

    public string Hierarchy { get; set; } = string.Empty;

    public string MarkdownFileName { get; set; } = string.Empty;

    public string MarkdownFilePath { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new List<string>();

    public string Content { get; set; } = string.Empty;

    public void AddItem(IItem item)
    {
        Items.Add(item);
        Items.OrderByDescending(x => x.Date);
    }
}
