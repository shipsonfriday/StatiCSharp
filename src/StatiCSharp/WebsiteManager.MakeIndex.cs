using StatiCSharp.Interfaces;
using System.Threading.Tasks;

namespace StatiCSharp;

public partial class WebsiteManager : IWebsiteManager
{
    /// <summary>
    /// Asynchronous creates and writes the index (homepage) of the website.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous index generating operation.</returns>
    private async Task MakeIndexAsync(OutputWriter output)
    {
            string body = HtmlFactory.MakeIndexHtml(Website.Index);
            string head = HtmlFactory.MakeHeadHtml();
            string index = HtmlDocument.Wrap(Website, Website.Index, head, _htmlBuilder.AdditionalHeaderContent, body);
            await output.WriteAsync(Output, "index.html", index);
            output.ClaimPath(Output);
        }
    }
