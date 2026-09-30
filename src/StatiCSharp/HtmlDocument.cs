using StatiCSharp.Interfaces;
using System;
using System.Globalization;
using System.Net;
using System.Text;

namespace StatiCSharp;

/// <summary>
/// Builds the html document around a rendered body: the doctype, the <c>&lt;head&gt;</c>
/// with the site's meta data, and the closing tags.
/// <para>
/// A pure function of its arguments. It used to be a method on the manager reaching into
/// the html builder for the parsers' head content; that content is now a parameter, so
/// what the document depends on is visible in the signature.
/// </para>
/// </summary>
internal static class HtmlDocument
{
    /// <summary>
    /// Wraps a body in the document around it.
    /// <para>
    /// The values taken from the site are meta data, so they are plain text and get
    /// encoded. <paramref name="themeHead"/>, <paramref name="parserHead"/> and
    /// <paramref name="body"/> are html already and are written through unchanged.
    /// </para>
    /// </summary>
    /// <param name="website">The website, which carries the language.</param>
    /// <param name="site">The site whose meta data goes into the head.</param>
    /// <param name="themeHead">Head content from the theme, from <c>MakeHeadHtml</c>.</param>
    /// <param name="parserHead">Head content the parsers need for their output to work.</param>
    /// <param name="body">The body, rendered by the theme.</param>
    /// <returns>The content for an html file.</returns>
    /// <exception cref="ArgumentNullException">An argument is null.</exception>
    internal static string Wrap(
        IWebsite website,
        ISite site,
        string themeHead,
        string parserHead,
        string body)
    {
        ArgumentNullException.ThrowIfNull(website);
        ArgumentNullException.ThrowIfNull(site);
        ArgumentNullException.ThrowIfNull(themeHead);
        ArgumentNullException.ThrowIfNull(parserHead);
        ArgumentNullException.ThrowIfNull(body);

        StringBuilder document = new();
        document.Append("<!doctype html>");
        document.Append(CultureInfo.InvariantCulture, $"<html lang=\"{WebUtility.HtmlEncode(website.Language.Name)}\">");
        document.Append("<head>");
        document.Append("<meta charset=\"utf-8\">");
        document.Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
        document.Append(CultureInfo.InvariantCulture, $"<title>{WebUtility.HtmlEncode(site.Title)}</title>");
        document.Append(CultureInfo.InvariantCulture, $"<meta name=\"description\" content=\"{WebUtility.HtmlEncode(site.Description)}\">");
        document.Append(CultureInfo.InvariantCulture, $"<meta name=\"author\" content=\"{WebUtility.HtmlEncode(site.Author)}\">");
        document.Append(CultureInfo.InvariantCulture, $"<meta name=\"keywords\" content=\"{WebUtility.HtmlEncode(string.Join(", ", site.Tags))}\">");
        document.Append("<link rel=\"icon\" type=\"image/x-icon\" href=\"/favicon.png\">");
        document.Append(themeHead);
        document.Append(parserHead);
        document.Append("</head>");
        document.Append(body);
        document.Append("</html>");

        return document.ToString();
    }
}
