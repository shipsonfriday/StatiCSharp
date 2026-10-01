using StatiCSharp.Interfaces;
using StatiCSharp.Tools;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StatiCSharp;

public partial class WebsiteManager : IWebsiteManager
{
    /// <summary>
    /// Asynchronous creates and writes the pages (not sections or items) of the website.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous pages generating operation.</returns>
    private async Task MakePagesAsync(OutputWriter output)
    {
        List<Task> tasks = new List<Task>();

        foreach (IPage site in Website.Pages)
        {
            tasks.Add(WritePage(site));
        }

        await Task.WhenAll(tasks);

        async Task WritePage(IPage site)
        {
            string defaultPath = FilenameToPath.From(site.MarkdownFileName);

            string pathInHierachy = (site.Path == string.Empty) ? defaultPath : site.Path;
            if (pathInHierachy == "index") { pathInHierachy = string.Empty; }

            await RenderSiteAsync(
                output,
                site,
                HtmlFactory.MakePageHtml(site),
                site.Hierarchy,
                pathInHierachy);
        }
    }
}
