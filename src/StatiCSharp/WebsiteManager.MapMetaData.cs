using StatiCSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static StatiCSharp.StatiCSharpConsole;

namespace StatiCSharp;

public partial class WebsiteManager : IWebsiteManager
{
    /// <summary>
    /// Adds the given meta data to a site (index, page, section or item).
    /// <para>
    /// A key that is absent, or present without a value, leaves the site's default in
    /// place. A value that is there but unusable is reported instead of being dropped.
    /// </para>
    /// </summary>
    /// <param name="metaData">The meta data.</param>
    /// <param name="site">The site where to add the meta data.</param>
    /// <exception cref="ArgumentNullException"><paramref name="metaData"/> or <paramref name="site"/> is null.</exception>
    internal void MapMetaData(Dictionary<string, string> metaData, ISite site)
    {
        ArgumentNullException.ThrowIfNull(metaData);
        ArgumentNullException.ThrowIfNull(site);

        // HtmlBuilder uses Markdown.ToHtml as the default parser, which adds <p>-marks at the beginning and end of each value. This is sliced manually every time for now. Trim() removes \n at the end of the string.
        if (TryRead("title", out string title))
        {
            site.Title = _htmlBuilder.ToHtml(title).Replace("<p>", "").Replace("</p>", "").Trim();
        }

        if (TryRead("description", out string description))
        {
            site.Description = _htmlBuilder.ToHtml(description).Replace("<p>", "").Replace("</p>", "").Trim();
        }

        if (TryRead("author", out string author))
        {
            site.Author = author;
        }

        if (TryRead("date", out string date))
        {
            // Invariant, because the documented format is ISO 8601. Parsing with the
            // current culture would read the same file differently on another machine.
            if (DateOnly.TryParse(date, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly parsed))
            {
                site.Date = parsed;
            }
            else
            {
                WriteLine($"WARNING: Could not read the date \"{date}\" in {site.MarkdownFilePath}. Expected ISO 8601, e.g. 2026-09-27. Using {site.Date:yyyy-MM-dd} instead.");
            }
        }

        if (TryRead("path", out string path))
        {
            site.Path = path;
        }

        if (TryRead("tags", out string tags))
        {
            site.Tags = tags
                .Split(',')
                .Select(tag => tag.Trim())
                .Where(tag => tag.Length > 0)
                .ToList();
        }

        // Absent key or empty value means "not given", so the default survives.
        bool TryRead(string key, out string value)
        {
            if (metaData.TryGetValue(key, out string? found) && !string.IsNullOrWhiteSpace(found))
            {
                value = found;
                return true;
            }

            value = string.Empty;
            return false;
        }
    }
}
