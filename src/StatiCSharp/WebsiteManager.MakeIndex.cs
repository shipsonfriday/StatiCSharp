using StatiCSharp.Interfaces;
using System.Threading.Tasks;

namespace StatiCSharp;

public partial class WebsiteManager : IWebsiteManager
{
    /// <summary>
    /// Asynchronous creates and writes the index (homepage) of the website.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous index generating operation.</returns>
    private async Task MakeIndexAsync()
    {
            string body = HtmlFactory.MakeIndexHtml(Website.Index);
            string head = HtmlFactory.MakeHeadHtml();
            string index = AddLeadingHtmlCode(Website, Website.Index, head, body);
            await WriteFileAsync(Output, "index.html", index, gitMode: GitMode);
            PathDirectory.Add(Output);
        }
    }
