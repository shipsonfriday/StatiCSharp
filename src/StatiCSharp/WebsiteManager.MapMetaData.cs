using StatiCSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StatiCSharp;

public partial class WebsiteManager : IWebsiteManager
{
    /// <summary>
    /// Adds the given meta data to a site (index, page, section or item). If there is no field for a given entry, it's sciped.
    /// </summary>
    /// <param name="metaData">The meta data.</param>
    /// <param name="site">The site where to add the meta data.</param>
    private void MapMetaData(Dictionary<string, string> metaData, ISite site)
    {
        // HtmlBuilder uses Markdown.ToHtml as the default parser, which adds <p>-marks at the beginning and end of each value. This is sliced manually every time for now. Trim() removes \n at the end of the string.
        try
        {
            if (metaData["title"] is not null)
            {
                site.Title = _htmlBuilder.ToHtml(metaData["title"]).Replace("<p>", "").Replace("</p>", "").Trim();
            }
        }
        catch { }

        try
        {
            if (metaData["description"] is not null)
            {
                site.Description = _htmlBuilder.ToHtml(metaData["description"]).Replace("<p>", "").Replace("</p>", "").Trim();
            }
        }
        catch { }

        try
        {
            if (metaData["author"] is not null)
            {
                site.Author = metaData["author"];
            }
        }
        catch { }

        try
        {
            if (metaData["date"] is not null)
            {
                site.Date = DateOnly.Parse(metaData["date"]);
            }
        }
        catch { }

        try
        {
            if (metaData["path"] is not null)
            {
                site.Path = metaData["path"];
            }
        }
        catch { }

        try
        {
            if (metaData["tags"] is not null)
            {
                site.Tags = metaData["tags"].Replace(" ", string.Empty).Split(',').ToList();
            }
        }
        catch { }
    }
}
