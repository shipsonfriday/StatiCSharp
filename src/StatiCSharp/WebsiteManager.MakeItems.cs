using StatiCSharp.Interfaces;
using StatiCSharp.Tools;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System;

namespace StatiCSharp;

public partial class WebsiteManager : IWebsiteManager
{
    /// <summary>
    /// Asynchronous creates and writes the items (not sections or pages) of the website.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous items generating operation.</returns>
    private async Task MakeItemsAsync()
    {
        List<Task> tasks = new List<Task>();

        foreach (ISection section in Website.Sections)
        {
            foreach(IItem site in section.Items)
            {
                tasks.Add(WriteItem(section, site));
            }
        }

        await Task.WhenAll(tasks);

        async Task WriteItem(ISection section, IItem site)
        {
            string body = HtmlFactory.MakeItemHtml(site);
            string head = HtmlFactory.MakeHeadHtml();
            string page = AddLeadingHtmlCode(Website, site, head, body);
            string defaultPath = FilenameToPath.From(site.MarkdownFileName);

            string itemPath = (site.Path != string.Empty) ? site.Path : defaultPath;
            string path = Directory.CreateDirectory(Path.Combine(Output, section.SectionName, itemPath)).ToString();

            if (PathDirectory.Contains(path))
            {
                Console.WriteLine($"WARNING: The path {path} is allready in use. Change the path in meta data to avoid duplicates.");
            }

            await WriteFileAsync(path: path, filename: "index.html", content: page, gitMode: GitMode);

            PathDirectory.Add(path);
        }
    }
}
