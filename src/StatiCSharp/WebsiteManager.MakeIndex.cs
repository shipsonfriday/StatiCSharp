using StatiCSharp.Interfaces;
using System.Threading.Tasks;

namespace StatiCSharp;

public partial class WebsiteManager : IWebsiteManager
{
    /// <summary>
    /// Asynchronous creates and writes the index (homepage) of the website.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous index generating operation.</returns>
    private Task MakeIndexAsync(OutputWriter output)
        => RenderSiteAsync(output, Website.Index, HtmlFactory.MakeIndexHtml(Website.Index));
}
