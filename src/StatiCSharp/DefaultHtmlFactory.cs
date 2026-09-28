using StatiCSharp.HtmlComponents;
using StatiCSharp.Interfaces;
using StatiCSharp.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

namespace StatiCSharp;

/// <summary>
/// Implementation of the default theme that's shipping with StatiC#.
/// </summary>
public class DefaultHtmlFactory: IHtmlFactory
{
    private int _numberOfArticlesOnHomepage = 10;

    /// <inheritdoc/>
    public string ResourcesPath
    {
        get {
            string? path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            return Path.Combine(path!, "DefaultResources");
        }
    }

    /// <summary>
    /// The website the theme is used for. So that the theme can access additional information.
    /// </summary>
    private IWebsite Website { get; set; }

    /// <summary>
    /// Initiate a new default theme, using the given website.
    /// </summary>
    /// <param name="website"></param>
    public DefaultHtmlFactory(IWebsite website)
    {
        Website = website;
    }

    /// <inheritdoc/>
    public string MakeHeadHtml()
    {
        return "<link rel=\"stylesheet\" href=\"/default-theme/styles.css\">";
    }

    /// <inheritdoc/>
    public string MakeIndexHtml(IIndex index)
    {
        // Collect all items to show, newest first, 10 items max.
        List<IItem> items = NewestFirst(Website.Sections.SelectMany(section => section.Items))
            .Take(_numberOfArticlesOnHomepage)
            .ToList();

        return new Body(
                new SiteHeader(Website),
                new Div(
                    new Div(index.Content).Class("welcomeWrapper"),
                    new H2("Latest Content"),
                    new ItemList(items, Website.Language)).Class("wrapper"),
                new Footer())
            .Render();
    }

    /// <inheritdoc/>
    public string MakePageHtml(IPage page)
    {
        return new Body(
                new SiteHeader(Website),
                new Div(
                    new Article(
                        new Div(page.Content).Class("content"))).Class("wrapper"),
                new Footer())
            .Render();
    }

    /// <inheritdoc/>
    public string MakeSectionHtml(ISection section)
    {
        List<IItem> items = NewestFirst(section.Items).ToList();
        return new Body(
                new SiteHeader(Website),
                new Div(section.Content).Class("wrapper"),
                new Div(
                    new ItemList(items, Website.Language)).Class("wrapper"),
                new Footer())
            .Render();
    }

    /// <inheritdoc/>
    public string MakeItemHtml(IItem item)
    {
        return new Body(
                new SiteHeader(Website),
                new Div(
                    new TagList(item.Tags),
                    new Text(FormatDate(item.Date, Website.Language))).Class("item-meta-data-header"),
                new Div(
                    new Article(
                        new Div(item.Content).Class("content"))).Class("wrapper"),
                new Footer())
            .Render();
    }

    /// <inheritdoc/>
    public string MakeTagListHtml(List<IItem> items, string tag)
    {
        return new Body(
                new SiteHeader(Website),
                new Div(
                    new H1(
                        new Text("Tagged with "),
                        new bigTag(tag)),
                    new ItemList(NewestFirst(items).ToList(), Website.Language)).Class("wrapper"),
                new Footer())
            .Render();
    }



    /// <summary>
    /// Orders items by date, newest first, without touching the given collection.
    /// <para>
    /// The previous version compared DateOnly values by converting both to a DateTime at
    /// a time parsed from the string "6pm" - a culture-dependent detour around a type
    /// that is directly comparable. It also used List.Sort, which is unstable, followed
    /// by Reverse, and it did so on the caller's own list: rendering a section reordered
    /// section.Items as a side effect.
    /// </para>
    /// </summary>
    private static IEnumerable<IItem> NewestFirst(IEnumerable<IItem> items)
        => items.OrderByDescending(item => item.Date);

    /// <summary>
    /// Wraps plain text for output. Text renders verbatim, because a site's Content is
    /// already html by then - meta data is not, so it has to be encoded here.
    /// </summary>
    private static Text Plain(string text) => new(WebUtility.HtmlEncode(text));

    /// <summary>
    /// Formats a date for display in the language the website declares.
    /// <para>
    /// Without an explicit culture the month name follows the machine that runs the
    /// generator, so an English site built on a German machine would read "März".
    /// </para>
    /// </summary>
    private static string FormatDate(DateOnly date, CultureInfo culture)
        => date.ToString("MMMM dd, yyyy", culture);


    ////////////
    /// Components
    ////////////
    
    private class SiteHeader : IHtmlComponent
    {
        List<string> sections;
        IWebsite website;
        public SiteHeader(IWebsite website)
        {
            this.website=website;
            this.sections = website.MakeSectionsFor;
        }
        public string Render()
        {
            Li[] navLinks = [.. sections.Select(section =>
                new Li(new A(Plain(section)).Href($"/{section}")))];

            return new Header(
                    new Div(
                        new A(Plain(this.website.Name)).Href("/").Class("site-name"),
                        new Nav(
                            new Ul(navLinks))).Class("wrapper"))
                .Render();
        }
    }

    private class ItemList: IHtmlComponent
    {
        private List<IItem> items;
        private CultureInfo culture;
        public ItemList(List<IItem> items, CultureInfo culture)
        {
            this.items = items;
            this.culture = culture;
        }
        public string Render()
        {
            Li[] listItems = [.. items.Select(item =>
                new Li(
                    new Article(
                        new H1(
                            new A(Plain(item.Title)).Href(item.Url)),
                        new Div(
                            new TagList(item.Tags),
                            new Text(FormatDate(item.Date, culture))).Class("item-meta-data"),
                        new Paragraph(Plain(item.Description)))))];

            return new Ul(listItems).Class("item-list").Render();
        }
    }

    private class TagList: IHtmlComponent
    {
        private List<string> tags;
        public TagList(List<string> tags)
        {
            this.tags = tags;
        }
        public string Render()
        {
            Li[] tagItems = [.. tags.Select(tag =>
                new Li(
                    new A(Plain(tag)).Href($"/tag/{UrlSlug.From(tag)}")).Class("variant-default"))];

            return new Ul(tagItems).Class("tags").Render();
        }
    }

    private class bigTag: IHtmlComponent
    {
        private string tag;
        public bigTag(string tag)
        {
            this.tag = tag;
        }
        public string Render()
        {
            var result = new Span(Plain(tag)).Class("tag");
            return result.Render();
        }
    }

    private class Footer: IHtmlComponent
    {
        public string Render()
        {
            return new HtmlComponents.Footer(
                    new Paragraph(
                        new Text("Generated with ❤️ using "),
                        new A("StatiC#").Href("https://github.com/RolandBraunDev/StatiCSharp")))
                .Render();
        }
    }
}
