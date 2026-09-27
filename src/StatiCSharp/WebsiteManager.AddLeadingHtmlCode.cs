using StatiCSharp.Interfaces;
using System;
using System.Net;
using System.Text;

namespace StatiCSharp;

public partial class WebsiteManager : IWebsiteManager
{
    /// <summary>
    /// Add the leading (and trailing) html code to a site.
    /// <para>
    /// The values taken from the site are meta data, so they are plain text and get
    /// encoded. <paramref name="head"/> and <paramref name="body"/> come from the theme
    /// and are html already, so they are written through unchanged.
    /// </para>
    /// </summary>
    /// <param name="website">The website, containing information for the head.</param>
    /// <param name="context">The site where the html-code should be added.</param>
    /// <param name="body">The body for the the, rendered by a HtmlFactory.</param>
    /// <param name="head">The additional content for the head of the site.</param>
    /// <returns>The content for a html-file as a string.</returns>
    /// <exception cref="ArgumentNullException">An argument is null.</exception>
    internal string AddLeadingHtmlCode(IWebsite website, ISite context, string head, string body)
    {
        ArgumentNullException.ThrowIfNull(website);
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(head);
        ArgumentNullException.ThrowIfNull(body);

        StringBuilder siteBuilder = new StringBuilder();
        siteBuilder.Append("<!doctype html>");
        siteBuilder.Append($"<html lang=\"{WebUtility.HtmlEncode(website.Language.Name)}\">");
        siteBuilder.Append("<head>");
        siteBuilder.Append("<meta charset=\"utf-8\">");
        siteBuilder.Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
        siteBuilder.Append($"<title>{WebUtility.HtmlEncode(context.Title)}</title>");
        siteBuilder.Append($"<meta name=\"description\" content=\"{WebUtility.HtmlEncode(context.Description)}\">");
        siteBuilder.Append($"<meta name=\"author\" content=\"{WebUtility.HtmlEncode(context.Author)}\">");
        siteBuilder.Append($"<meta name=\"keywords\" content=\"{WebUtility.HtmlEncode(string.Join(", ", context.Tags))}\">");
        siteBuilder.Append("<link rel=\"icon\" type=\"image/x-icon\" href=\"/favicon.png\">");
        siteBuilder.Append(head);
        siteBuilder.Append(_htmlBuilder.AdditionalHeaderContent);
        siteBuilder.Append("</head>");
        siteBuilder.Append(body);
        siteBuilder.Append("</html>");

        return siteBuilder.ToString();
    }
}
