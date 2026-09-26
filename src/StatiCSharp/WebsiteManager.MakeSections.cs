using StatiCSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace StatiCSharp;

public partial class WebsiteManager : IWebsiteManager
{
    /// <summary>
    /// Asynchronously creates and writes the sections (not pages or items) of the website.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous sections generating operation.</returns>
    private async Task MakeSectionsAsync()
    {
        List<Task> tasks = new List<Task>();

        foreach (ISection site in Website.Sections)
        {
            tasks.Add(WriteSection(site));
        }

        await Task.WhenAll(tasks);

        async Task WriteSection(ISection site)
        {
            string body = HtmlFactory.MakeSectionHtml(site);
            string head = HtmlFactory.MakeHeadHtml();
            string page = AddLeadingHtmlCode(Website, site, head, body);
            string path = Directory.CreateDirectory(Path.Combine(Output, site.SectionName)).ToString();

            if (PathDirectory.Contains(path))
            {
                Console.WriteLine($"WARNING: The path {path} is allready in use. Change the path in meta data to avoid duplicates.");
            }

            await WriteFileAsync(path: path, filename: "index.html", content: page, gitMode: GitMode);

            PathDirectory.Add(path);
        }
    }
}
