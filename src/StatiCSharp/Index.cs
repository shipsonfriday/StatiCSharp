using StatiCSharp.Interfaces;
using System;
using System.Collections.Generic;

namespace StatiCSharp;

/// <summary>
/// Represenation of the index page.
/// </summary>
internal class Index : IIndex
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    public DateOnly DateLastModified { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    public string Path { get; set; } = string.Empty;

    public string Url
    {
        get { return "/"; }
    }

    public string MarkdownFileName { get; set; } = string.Empty;

    public string MarkdownFilePath { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new List<string>();

    public string Content { get; set; } = string.Empty;
}
