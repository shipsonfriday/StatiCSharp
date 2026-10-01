using StatiCSharp.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            await RenderSiteAsync(output, site, HtmlFactory.MakeSectionHtml(site), site.SectionName);
        }
    }
}
