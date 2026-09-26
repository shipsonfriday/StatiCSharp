using StatiCSharp.Interfaces;
using StatiCSharp.Tools;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System;

namespace StatiCSharp;

public partial class WebsiteManager : IWebsiteManager
{
    /// <summary>
    /// Asynchronous creates and writes the pages (not sections or items) of the website.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous pages generating operation.</returns>
    private async Task MakePagesAsync()
    {
        List<Task> tasks = new List<Task>();

        foreach (IPage site in Website.Pages)
        {
            tasks.Add(WritePage(site));
        }

        await Task.WhenAll(tasks);

        async Task WritePage(IPage site)
        {
            string body = HtmlFactory.MakePageHtml(site);
            string head = HtmlFactory.MakeHeadHtml();
            string page = AddLeadingHtmlCode(Website, site, head, body);
            string defaultPath = FilenameToPath.From(site.MarkdownFileName);

            // Create directory, if it does not excist.
            string pathInHierachy = (site.Path == string.Empty) ? defaultPath : site.Path;
            if (pathInHierachy == "index") { pathInHierachy = string.Empty; }
            string path = Directory.CreateDirectory(Path.Combine(Output, site.Hierarchy, pathInHierachy)).ToString();

            if (PathDirectory.Contains(path))
            {
                Console.WriteLine($"WARNING: The path {path} is allready in use. Change the path in meta data to avoid duplicates.");
            }

            await WriteFileAsync(path: path, filename: "index.html", content: page, gitMode: GitMode);
            PathDirectory.Add(path);
        }
    }
}
