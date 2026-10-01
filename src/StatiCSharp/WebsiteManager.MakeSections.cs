using StatiCSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using static StatiCSharp.StatiCSharpConsole;

namespace StatiCSharp;

public partial class WebsiteManager : IWebsiteManager
{
    /// <summary>
    /// Asynchronously creates and writes the sections (not pages or items) of the website.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous sections generating operation.</returns>
    private async Task MakeSectionsAsync(OutputWriter output)
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
            string page = HtmlDocument.Wrap(Website, site, head, _htmlBuilder.AdditionalHeaderContent, body);
            string path = Directory.CreateDirectory(Path.Combine(Output, site.SectionName)).ToString();

            if (!await output.WriteAsync(path, "index.html", page))
            {
                WriteLine($"WARNING: The path {path} is already in use. Change the path in meta data to avoid duplicates.");
            }

        }
    }
}
