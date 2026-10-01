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

    /// <inheritdoc/>
    public string MakeHeadHtml(ISite site, RenderContext context)
    {
        return new Link().Rel("stylesheet").Href("/default-theme/styles.css").Render();
    }

    /// <inheritdoc/>
    public string MakeIndexHtml(IIndex index, RenderContext context)
    {
        // Collect all items to show, newest first, 10 items max.
        List<IItem> items = NewestFirst(context.Sections.SelectMany(section => section.Items))
            .Take(_numberOfArticlesOnHomepage)
            .ToList();

        return new Body(
                new SiteHeader(context),
                new Div(
                    new Div(index.Content).Class("welcomeWrapper"),
                    new H2("Latest Content"),
                    new ItemList(items, context.Website.Language)).Class("wrapper"),
                new Footer())
            .Render();
    }

    /// <inheritdoc/>
    public string MakePageHtml(IPage page, RenderContext context)
    {
        return new Body(
                new SiteHeader(context),
                new Div(
                    new Article(
                        new Div(page.Content).Class("content"))).Class("wrapper"),
                new Footer())
            .Render();
    }

    /// <inheritdoc/>
    public string MakeSectionHtml(ISection section, RenderContext context)
    {
        List<IItem> items = NewestFirst(section.Items).ToList();
        return new Body(
                new SiteHeader(context),
                new Div(section.Content).Class("wrapper"),
                new Div(
                    new ItemList(items, context.Website.Language)).Class("wrapper"),
                new Footer())
            .Render();
    }

    /// <inheritdoc/>
    public string MakeItemHtml(IItem item, RenderContext context)
    {
        return new Body(
                new SiteHeader(context),
                new Div(
                    new TagList(item.Tags),
                    new Text(FormatDate(item.Date, context.Website.Language))).Class("item-meta-data-header"),
                new Div(
                    new Article(
                        new Div(item.Content).Class("content"))).Class("wrapper"),
                new Footer())
            .Render();
    }

    /// <inheritdoc/>
    public string MakeTagListHtml(List<IItem> items, string tag, RenderContext context)
    {
        return new Body(
                new SiteHeader(context),
                new Div(
                    new H1(
                        new Text("Tagged with "),
                        new bigTag(tag)),
                    new ItemList(NewestFirst(items).ToList(), context.Website.Language)).Class("wrapper"),
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
        private readonly RenderContext context;

        public SiteHeader(RenderContext context)
        {
            this.context = context;
        }

        public string Render()
        {
            // Built from the sections that were read, not from the folder names that were
            // configured. That gives the title from the section's index.md instead of the
            // folder name, and a section that was named but has no index.md no longer gets a
            // link to a page that does not exist.
            Li[] navLinks = [.. context.Sections.Select(section =>
                new Li(new A(Plain(NameOf(section))).Href(section.Url)))];

            return new Header(
                    new Div(
                        new A(Plain(this.context.Website.Name)).Href("/").Class("site-name"),
                        new Nav(
                            new Ul(navLinks))).Class("wrapper"))
                .Render();
        }

        /// <summary>
        /// What the link says: the section's title, or its folder name when the index.md
        /// carries no title - an empty link would be unclickable.
        /// </summary>
        private static string NameOf(ISection section)
            => section.Title.Length > 0 ? section.Title : section.SectionName;
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
