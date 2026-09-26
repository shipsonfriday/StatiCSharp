using StatiCSharp.HtmlComponents;
using StatiCSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

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
        // Collect all items to show. 10 items max.
        List<IItem> items = new List<IItem>();
        foreach (ISection section in Website.Sections)
            {
                section.Items.ForEach((item) => items.Add(item));
            }
        int showArticles = (items.Count > _numberOfArticlesOnHomepage) ? _numberOfArticlesOnHomepage : items.Count;
        items.Sort((i1, i2) => DateTime.Compare(i1.Date.ToDateTime(TimeOnly.Parse("6pm")), i2.Date.ToDateTime(TimeOnly.Parse("6pm"))));
        items.Reverse();
        items = items.GetRange(0, showArticles);

        return  new Body()  .Add(new SiteHeader(Website))
                            .Add(new Div()
                                .Add(new Div(index.Content)
                                        .Class("welcomeWrapper"))
                                .Add(new H2("Latest Content"))
                                .Add(new ItemList(items))
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
        List<IItem> items = section.Items;
        items.Sort( (i1, i2) => DateTime.Compare(i1.Date.ToDateTime(TimeOnly.Parse("6pm")), i2.Date.ToDateTime(TimeOnly.Parse("6pm"))));
        items.Reverse();
        return new Body()   .Add(new SiteHeader(Website))
                            .Add(new Div(section.Content)
                                .Class("wrapper"))
                            .Add(new Div()
                                .Add(new ItemList(items))
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
                                .Add(new Text(item.Date.ToString("MMMM dd, yyyy")))
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
        items.Sort( (i1, i2) => DateTime.Compare(i1.Date.ToDateTime(TimeOnly.Parse("6pm")), i2.Date.ToDateTime(TimeOnly.Parse("6pm"))));
        items.Reverse();
        return new Body()   .Add(new SiteHeader(Website))
                            .Add(new Div()
                                .Add(new H1()
                                    .Add(new Text("Tagged with "))
                                    .Add(new bigTag(tag)))
                                .Add(new ItemList(items))
                                .Class("wrapper"))
                            .Add(new Footer())
                .Render();
    }



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
                    NavLinks.Add(new Li(new A(section).Href($"/{section}")));
                }
            }
            return new Header(
                            new Div(
                                new A(this.website.Name).Href("/").Class("site-name")
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
        public ItemList(List<IItem> items)
        {
            this.items = items;
        }
        public string Render()
        {
            var result = new Ul().Class("item-list");
            items.ForEach((item) => result.Add(
                                            new Li()
                                                .Add(new Article()
                                                    .Add(new H1().Add(
                                                                new A(item.Title).Href(item.Url)
                                                            )
                                                    )
                                                    .Add(new Div()
                                                            .Add(new TagList(item.Tags))
                                                            .Add(new Text(item.Date.ToString("MMMM dd, yyyy")))
                                                            .Class("item-meta-data"))
                                                    .Add(new Text($"<p>{item.Description}</p>"))
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
                                                    .Add(new A(tag).Href($"/tag/{tag}")))
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
            var result = new Span(tag).Class("tag");
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
