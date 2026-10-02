using StatiCSharp.Interfaces;
using StatiCSharp.Tools;

namespace StatiCSharp;

/// <summary>
/// Turns a website that has been read into the html files that make it up: the index, the
/// pages, the sections, their items and a list per tag.
/// <para>
/// The counterpart of <see cref="ContentReader"/>. Where the reader makes a model out of
/// files, this makes files out of the model, and what it needs to do that stands in its
/// constructor instead of being reached for on the manager.
/// </para>
/// </summary>
internal sealed class WebsiteRenderer
{
    private readonly RenderContext _context;
    private readonly IHtmlFactory _htmlFactory;
    private readonly HtmlBuilder _htmlBuilder;
    private readonly OutputWriter _output;
    private readonly Action<string> _log;

    /// <summary>
    /// Starts a renderer for one run.
    /// </summary>
    /// <param name="context">The website's configuration together with the content that was read.</param>
    /// <param name="htmlFactory">The theme that renders the bodies.</param>
    /// <param name="htmlBuilder">The pipeline, for the head content its parsers need.</param>
    /// <param name="output">The writer for this run, which knows where the output goes.</param>
    /// <param name="log">Where a message about a site goes. May be called from several threads.</param>
    /// <exception cref="ArgumentNullException">An argument is null.</exception>
    internal WebsiteRenderer(
        RenderContext context,
        IHtmlFactory htmlFactory,
        HtmlBuilder htmlBuilder,
        OutputWriter output,
        Action<string> log)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(htmlFactory);
        ArgumentNullException.ThrowIfNull(htmlBuilder);
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(log);

        // The same instance is handed to every call, so the theme sees one website throughout
        // the run and does not have to hold anything itself.
        _context = context;
        _htmlFactory = htmlFactory;
        _htmlBuilder = htmlBuilder;
        _output = output;
        _log = log;
    }

    /// <summary>
    /// Renders the index (homepage) of the website.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous index generating operation.</returns>
    internal Task RenderIndexAsync()
        => RenderSiteAsync(_context.Index, _htmlFactory.MakeIndexHtml(_context.Index, _context));

    // Nothing below decides where a site goes any more: RenderSiteAsync reads that off the
    // site's own url.

    /// <summary>
    /// Renders the pages (not sections or items) of the website.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous pages generating operation.</returns>
    internal async Task RenderPagesAsync()
    {
        List<Task> tasks = new List<Task>();

        foreach (IPage site in _context.Pages)
        {
            tasks.Add(WritePage(site));
        }

        await Task.WhenAll(tasks);

        async Task WritePage(IPage site)
        {
            await RenderSiteAsync(site, _htmlFactory.MakePageHtml(site, _context));
        }
    }

    /// <summary>
    /// Renders the sections (not pages or items) of the website.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous sections generating operation.</returns>
    internal async Task RenderSectionsAsync()
    {
        List<Task> tasks = new List<Task>();

        foreach (ISection site in _context.Sections)
        {
            tasks.Add(WriteSection(site));
        }

        await Task.WhenAll(tasks);

        async Task WriteSection(ISection site)
        {
            await RenderSiteAsync(site, _htmlFactory.MakeSectionHtml(site, _context));
        }
    }

    /// <summary>
    /// Renders the items (not sections or pages) of the website.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous items generating operation.</returns>
    internal async Task RenderItemsAsync()
    {
        List<Task> tasks = new List<Task>();

        foreach (ISection section in _context.Sections)
        {
            foreach (IItem site in section.Items)
            {
                tasks.Add(WriteItem(site));
            }
        }

        await Task.WhenAll(tasks);

        async Task WriteItem(IItem site)
        {
            await RenderSiteAsync(site, _htmlFactory.MakeItemHtml(site, _context));
        }
    }

    /// <summary>
    /// Renders the tag pages of the website.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous tags generating operation.</returns>
    internal async Task RenderTagListsAsync()
    {
        // Collect all available tags
        List<string> tags = new List<string>();
        foreach (ISection currentSection in _context.Sections)
        {
            foreach (IItem currentItem in currentSection.Items)
            {
                foreach (string tag in currentItem.Tags)
                {
                    // Check if tag is already in list. If not, add it
                    if (!tags.Contains(tag)) { tags.Add(tag); }
                }
            }
        }

        // Two tags can share a slug, e.g. "Web Dev" and "web-dev". They would be written
        // to the same directory, so say so rather than letting one overwrite the other.
        foreach (var collision in tags.GroupBy(UrlSlug.From).Where(group => group.Count() > 1))
        {
            _log($"WARNING: The tags {string.Join(", ", collision.Select(tag => $"\"{tag}\""))} all lead to /tag/{collision.Key}. Only one of them will be written.");
        }

        List<Task> tasks = new List<Task>();

        foreach (string tag in tags)
        {
            tasks.Add(WriteTagList(tag));
        }

        await Task.WhenAll(tasks);

        async Task WriteTagList(string tag)
        {
            string slug = UrlSlug.From(tag);

            if (slug.Length == 0)
            {
                // Nothing usable is left, e.g. for a tag written as "+++". An empty segment
                // would put the tag page into the /tag directory itself.
                _log($"WARNING: The tag \"{tag}\" has no characters that can be used in a url. Skipping it.");
                return;
            }

            List<IItem> itemsWithCurrentTag = new();
            // Collect all items with the current tag
            foreach (ISection currentSection in _context.Sections)
            {
                foreach (IItem item in currentSection.Items)
                {
                    if (item.Tags.Contains(tag))
                    {
                        itemsWithCurrentTag.Add(item);
                    }
                }
            }

            // A tag page is a site like any other, so it says where it goes: /tag/<slug>.
            Item tagPage = new()
            {
                Title = $"{tag} | {_context.Website.Name}",
                Section = "tag",
                Path = slug,
            };

            await RenderSiteAsync(
                tagPage,
                _htmlFactory.MakeTagListHtml(itemsWithCurrentTag, tag, _context));
        }
    }

    /// <summary>
    /// Writes one site into the output: wraps the body the theme rendered in the html
    /// document around it, makes sure the target directory exists, and writes index.html.
    /// <para>
    /// The index, the pages, the sections, the items and the tag lists all did exactly this,
    /// each with its own copy. What actually differs between them is which theme method
    /// produces the body, so that is all they say.
    /// </para>
    /// </summary>
    /// <param name="site">The site, whose meta data goes into the document head and whose url says where it goes.</param>
    /// <param name="body">The body, rendered by the theme.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous write operation.</returns>
    private async Task RenderSiteAsync(ISite site, string body)
    {
        string document = HtmlDocument.Wrap(
            _context.Website,
            site,
            _htmlFactory.MakeHeadHtml(site, _context),
            _htmlBuilder.AdditionalHeaderContent,
            body);

        // The url says where the site goes; the writer turns that into a directory below the
        // output. There used to be a second path calculation here, kept in step with the Url
        // properties by a comment asking for it, which is how a path of "../.." in the front
        // matter could write outside the output directory.
        if (!await _output.WriteAsync(UrlPath.SegmentsOf(site.Url), "index.html", document))
        {
            _log($"WARNING: Two sites are written to {site.Url}. Change the path in the meta data of one of them; only the last one is kept.");
        }
    }
}
