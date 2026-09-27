using StatiCSharp.HtmlComponents;
using StatiCSharp.Interfaces;
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

        return  new Body()  .Add(new SiteHeader(Website))
                            .Add(new Div()
                                .Add(new Div(index.Content)
                                        .Class("welcomeWrapper"))
                                .Add(new H2("Latest Content"))
                                .Add(new ItemList(items, Website.Language))
                                .Class("wrapper"))
                            .Add(new Footer())
                .Render();
    }

    /// <inheritdoc/>
    public string MakePageHtml(IPage page)
    {
        return new Body()   .Add(new SiteHeader(Website))
                            .Add(new Div()
                                .Add(new Article()
                                    .Add(new Div(page.Content)
                                        .Class("content")))
                                .Class("wrapper"))
                            .Add(new Footer())
                .Render();
    }

    /// <inheritdoc/>
    public string MakeSectionHtml(ISection section)
    {
        List<IItem> items = NewestFirst(section.Items).ToList();
        return new Body()   .Add(new SiteHeader(Website))
                            .Add(new Div(section.Content)
                                .Class("wrapper"))
                            .Add(new Div()
                                .Add(new ItemList(items, Website.Language))
                                .Class("wrapper"))
                            .Add(new Footer())
                .Render();
    }

    /// <inheritdoc/>
    public string MakeItemHtml(IItem item)
    {
        return new Body()   .Add(new SiteHeader(Website))
                            .Add(new Div()
                                .Add(new TagList(item.Tags))
                                .Add(new Text(FormatDate(item.Date, Website.Language)))
                                .Class("item-meta-data-header"))
                            .Add(new Div()
                                .Add(new Article()
                                    .Add(new Div(item.Content)
                                        .Class("content")))
                                .Class("wrapper"))
                            .Add(new Footer())
                .Render();
    }

    /// <inheritdoc/>
    public string MakeTagListHtml(List<IItem> items, string tag)
    {
        return new Body()   .Add(new SiteHeader(Website))
                            .Add(new Div()
                                .Add(new H1()
                                    .Add(new Text("Tagged with "))
                                    .Add(new bigTag(tag)))
                                .Add(new ItemList(NewestFirst(items).ToList(), Website.Language))
                                .Class("wrapper"))
                            .Add(new Footer())
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
            Ul NavLinks = new();
            foreach (var section in sections)
            {
                if (section.ToString() is not null)
                {
                    NavLinks.Add(new Li(new A(Plain(section)).Href($"/{section}")));
                }
            }
            return new Header(
                            new Div(
                                new A(Plain(this.website.Name)).Href("/").Class("site-name")
                            ).Add(
                                new Nav().Add(
                                    new Ul().Add(NavLinks)
                                )
                            ).Class("wrapper")
                    )
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
            var result = new Ul().Class("item-list");
            items.ForEach((item) => result.Add(
                                            new Li()
                                                .Add(new Article()
                                                    .Add(new H1().Add(
                                                                new A(Plain(item.Title)).Href(item.Url)
                                                            )
                                                    )
                                                    .Add(new Div()
                                                            .Add(new TagList(item.Tags))
                                                            .Add(new Text(FormatDate(item.Date, culture)))
                                                            .Class("item-meta-data"))
                                                    .Add(new Paragraph(Plain(item.Description)))
                                                )
                                            )
                                    );
            return result.Render();
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
            var result = new Ul().Class("tags");
            tags.ForEach((tag) => result.Add(
                                            new Li().Class("variant-default")
                                                    .Add(new A(Plain(tag)).Href($"/tag/{tag}")))
                        );
            return result.Render();
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
            return new HtmlComponents.Footer()
            .Add(new Paragraph()
                    .Add(new Text("Generated with ❤️ using "))
                    .Add(new A("StatiC#").Href("https://github.com/RolandBraunDev/StatiCSharp")))
            .Render();
        }
    }
}
