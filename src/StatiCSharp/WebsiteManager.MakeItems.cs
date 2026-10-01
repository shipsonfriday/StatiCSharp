using StatiCSharp.Interfaces;
using StatiCSharp.Tools;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StatiCSharp;

public partial class WebsiteManager : IWebsiteManager
{
    /// <summary>
    /// Asynchronous creates and writes the items (not sections or pages) of the website.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous items generating operation.</returns>
    private async Task MakeItemsAsync(OutputWriter output)
    {
        List<Task> tasks = new List<Task>();

        foreach (ISection section in Website.Sections)
        {
            foreach (IItem site in section.Items)
            {
                tasks.Add(WriteItem(section, site));
            }
        }

        await Task.WhenAll(tasks);

        async Task WriteItem(ISection section, IItem site)
        {
            string defaultPath = FilenameToPath.From(site.MarkdownFileName);
            string itemPath = (site.Path != string.Empty) ? site.Path : defaultPath;

            await RenderSiteAsync(
                output,
                site,
                HtmlFactory.MakeItemHtml(site),
                section.SectionName,
                itemPath);
        }
    }
}
