using StatiCSharp.Interfaces;
using System.IO;
using System.Threading.Tasks;
using static StatiCSharp.StatiCSharpConsole;

namespace StatiCSharp;

public partial class WebsiteManager : IWebsiteManager
{
    /// <summary>
    /// Writes one site into the output: wraps the body the theme rendered in the html
    /// document around it, makes sure the target directory exists, and writes index.html.
    /// <para>
    /// The index, the pages, the sections, the items and the tag lists all did exactly this,
    /// each with its own copy. What actually differs between them is which theme method
    /// produces the body and where the site goes, so that is what they still say.
    /// </para>
    /// </summary>
    /// <param name="output">The writer for this run.</param>
    /// <param name="site">The site, whose meta data goes into the document head.</param>
    /// <param name="body">The body, rendered by the theme.</param>
    /// <param name="pathSegments">Where the site goes, below the output directory.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous write operation.</returns>
    private async Task RenderSiteAsync(OutputWriter output, ISite site, string body, params string[] pathSegments)
    {
        string path = Directory.CreateDirectory(Path.Combine([Output, .. pathSegments])).ToString();

        string document = HtmlDocument.Wrap(
            Website,
            site,
            HtmlFactory.MakeHeadHtml(),
            _htmlBuilder.AdditionalHeaderContent,
            body);

        if (!await output.WriteAsync(path, "index.html", document))
        {
            WriteLine($"WARNING: The path {path} is already in use. Change the path in meta data to avoid duplicates.");
        }
    }
}
