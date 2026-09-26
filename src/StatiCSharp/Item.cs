using StatiCSharp.Interfaces;
using StatiCSharp.Tools;
using System;
using System.Collections.Generic;

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
    /// The relative url of the item.
    /// <para>
    /// Derives the path segment through <see cref="FilenameToPath"/>, the same way
    /// WebsiteManager.MakeItems derives the output directory. Both sides have to agree,
    /// or every generated link points at a directory that is not there.
    /// </para>
    /// </summary>
    public string Url
    {
        get
        {
            string segment = string.IsNullOrEmpty(Path)
                ? FilenameToPath.From(MarkdownFileName)
                : Path;

            return $"/{Section}/{segment}";
        }
    }

    public string Section { get; set; } = string.Empty;

    public string MarkdownFileName { get; set; } = string.Empty;

    public string MarkdownFilePath { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new List<string>();

    public string Content { get; set; } = string.Empty;
}
