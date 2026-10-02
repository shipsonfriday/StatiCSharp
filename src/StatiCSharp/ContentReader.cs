using StatiCSharp.Interfaces;
using StatiCSharp.Tools;
using System.Globalization;
using static StatiCSharp.StatiCSharpConsole;

namespace StatiCSharp;

/// <summary>
/// Reads the markdown files in the content directory and returns what it finds: the index, the
/// pages, the sections and their items.
/// <para>
/// The counterpart of <see cref="OutputWriter"/>. The manager used to do both, which is why
/// reading and writing shared fields and the order of the two was a matter of reading
/// <c>MakeAsync</c> from top to bottom.
/// </para>
/// <para>
/// It used to fill the website that was passed in. That made a website both the configuration
/// its author wrote and the result of reading, so a second run had to start by emptying it,
/// the index could not be replaced and a theme could not tell the two apart. Reading returns
/// a new result now and touches nothing.
/// </para>
/// </summary>
internal sealed class ContentReader
{
    private readonly string _contentDirectory;
    private readonly HtmlBuilder _htmlBuilder;

    /// <summary>
    /// Starts a reader for one content directory.
    /// </summary>
    /// <param name="contentDirectory">The absolute path of the directory holding the markdown files.</param>
    /// <param name="htmlBuilder">The pipeline that turns the markdown content into html.</param>
    /// <exception cref="ArgumentNullException">An argument is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="contentDirectory"/> is empty or only whitespace.</exception>
    internal ContentReader(string contentDirectory, HtmlBuilder htmlBuilder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(contentDirectory);
        ArgumentNullException.ThrowIfNull(htmlBuilder);

        _contentDirectory = contentDirectory;
        _htmlBuilder = htmlBuilder;
    }

    /// <summary>
    /// Reads the content directory and returns it together with the given configuration, ready
    /// to be rendered.
    /// </summary>
    /// <param name="website">The configuration of the website. It is only read.</param>
    /// <returns>The index, the pages and the sections that were found.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="website"/> is null.</exception>
    internal RenderContext Read(IWebsite website)
    {
        ArgumentNullException.ThrowIfNull(website);

        Index index = new();
        List<IPage> pages = [];
        List<ISection> sections = [];

        string[] directoriesOfContent = Directory.GetDirectories(_contentDirectory);

        // Index. Stays empty when there is no index.md.
        string pathOfIndex = Path.Combine(_contentDirectory, "index.md");
        if (File.Exists(pathOfIndex))
            FillFromMarkdown(index, pathOfIndex);


        // Pages
        foreach (string directory in directoriesOfContent)
        {
            string nameOfCurrentDirectory = Path.GetFileName(directory);
            if (!website.MakeSectionsFor.Contains(nameOfCurrentDirectory))
            {
                ProcessAllPagesInDirectory(directory);
            }
        }

        void ProcessAllPagesInDirectory(string dir)
        {
            string[] files = Directory.GetFiles(dir);
            var dirs = Directory.GetDirectories(dir);

            foreach (string file in files)
            {
                if (IsMarkdownFile(file))
                {
                    Page page = ReadPage(file);

                    // An index.md is the page of its folder, so having no segment of its own
                    // is exactly right for it.
                    if (IsIndexFile(file) || HasItsOwnSegment(page, file))
                    {
                        pages.Add(page);
                    }
                }
            }

            foreach (string directory in dirs)
            {
                ProcessAllPagesInDirectory(directory);
            }
        }


        // Sections, in the order the author named them rather than the order the file system
        // happens to return the directories in. A navigation built from them is then the
        // navigation that was asked for.
        //
        // Matched against the directories that are actually there, not by building the path
        // from the name: on a case insensitive file system "Posts" would otherwise find the
        // folder "posts", while the loop above - comparing exactly - would have read the same
        // files as pages already. Distinct, because a name said twice is still one section.
        foreach (string sectionName in website.MakeSectionsFor.Distinct(StringComparer.Ordinal))
        {
            string? directory = directoriesOfContent.FirstOrDefault(
                candidate => Path.GetFileName(candidate).Equals(sectionName, StringComparison.Ordinal));

            if (directory is null)
            {
                continue;
            }

            string pathOfSectionIndexFile = Path.Combine(directory, "index.md");

            if (!File.Exists(pathOfSectionIndexFile))
            {
                continue;
            }

            // A folder name of nothing but symbols leaves no url segment, so the section would
            // be written over the index of the website.
            if (!UrlPath.HasASegment(sectionName))
            {
                WriteLine($"WARNING: The section folder \"{sectionName}\" has no characters that can be used in a url. Skipping it.");
                continue;
            }

            sections.Add(ReadSection(pathOfSectionIndexFile));
        }

        return new RenderContext
        {
            Website = website,
            Index = index,
            Pages = pages,
            Sections = sections,
        };
    }

    /// <summary>
    /// Reads a page.
    /// </summary>
    private Page ReadPage(string path)
    {
        Page page = new()
        {
            Hierarchy = HierarchyOf(path),
        };

        FillFromMarkdown(page, path);

        return page;
    }

    /// <summary>
    /// Reads a section from its index file, together with every item in the same directory.
    /// </summary>
    private Section ReadSection(string path)
    {
        string sectionFolder = Path.GetDirectoryName(path)!;

        Section section = new()
        {
            SectionName = Path.GetFileName(sectionFolder),
        };

        FillFromMarkdown(section, path);

        foreach (string itemFile in Directory.GetFiles(sectionFolder))
        {
            if (IsMarkdownFile(itemFile) && !IsIndexFile(itemFile))
            {
                IItem item = ReadItem(itemFile, section.SectionName);

                if (HasItsOwnSegment(item, itemFile))
                {
                    section.AddItem(item);
                }
            }
        }

        return section;
    }

    /// <summary>
    /// Whether the site read from this file gets a url segment of its own. Without one it would
    /// be written over the page in the directory above it, so it is reported and left out.
    /// </summary>
    private static bool HasItsOwnSegment(ISite site, string path)
    {
        if (UrlPath.HasASegment(SiteSegment.Of(site.Path, site.MarkdownFileName)))
        {
            return true;
        }

        WriteLine($"WARNING: {path} has no characters that can be used in a url, neither in its filename nor in its path entry. Skipping it.");
        return false;
    }

    /// <summary>
    /// Reads one item of a section.
    /// </summary>
    private IItem ReadItem(string path, string sectionName)
    {
        DateOnly lastModified = DateOnly.FromDateTime(File.GetLastWriteTime(path));

        IItem item = new Item
        {
            Section = sectionName,
            DateLastModified = lastModified,

            // Fallback for items whose meta data carries no date. Set before filling, because
            // MapMetaData leaves the value in place when no date is given - no need to route
            // the date through the dictionary as a string, which also made it culture
            // dependent.
            Date = lastModified,
        };

        FillFromMarkdown(item, path);

        return item;
    }

    /// <summary>
    /// Fills what every site has, whatever kind it is: the content, where it came from, and
    /// the meta data from the front matter.
    /// <para>
    /// The order matters. <c>MarkdownFilePath</c> is set before the meta data is mapped,
    /// because a warning about an unusable value names the file it came from, and a value
    /// already on the site survives mapping when the front matter does not give one.
    /// </para>
    /// </summary>
    private void FillFromMarkdown(ISite site, string path)
    {
        MarkdownFile file = MarkdownFactory.Read(path);

        site.Content = _htmlBuilder.ToHtml(file.Content);
        site.MarkdownFileName = Path.GetFileName(path);
        site.MarkdownFilePath = path;

        MapMetaData(file.MetaData, site);
    }

    /// <summary>
    /// The directories between the content directory and the file, e.g. "docs/guide" for
    /// a page at "Content/docs/guide/a-page.md". That is where the page is written to.
    /// </summary>
    private string HierarchyOf(string path)
    {
        string hierarchy = path.Remove(0, _contentDirectory.Length);

        if (hierarchy[0] == '/' || hierarchy[0] == '\\')
        {
            hierarchy = hierarchy.Remove(0, 1);
        }

        return Path.GetDirectoryName(hierarchy)!;
    }

    /// <summary>
    /// Adds the given meta data to a site (index, page, section or item).
    /// <para>
    /// A key that is absent, or present without a value, leaves the site's default in
    /// place. A value that is there but unusable is reported instead of being dropped.
    /// </para>
    /// <para>
    /// Every value is taken as plain text. Only the content below the front matter is
    /// markdown; title and description are encoded where they are rendered.
    /// </para>
    /// </summary>
    /// <param name="metaData">The meta data.</param>
    /// <param name="site">The site where to add the meta data.</param>
    /// <exception cref="ArgumentNullException"><paramref name="metaData"/> or <paramref name="site"/> is null.</exception>
    internal static void MapMetaData(Dictionary<string, string> metaData, ISite site)
    {
        ArgumentNullException.ThrowIfNull(metaData);
        ArgumentNullException.ThrowIfNull(site);

        // Plain text, not markup. These end up in <title> and in meta content
        // attributes, where markup does not belong, and the render sites encode them.
        if (TryRead("title", out string title))
        {
            site.Title = title;
        }

        if (TryRead("description", out string description))
        {
            site.Description = description;
        }

        if (TryRead("author", out string author))
        {
            site.Author = author;
        }

        if (TryRead("date", out string date))
        {
            // Invariant, because the documented format is ISO 8601. Parsing with the
            // current culture would read the same file differently on another machine.
            if (DateOnly.TryParse(date, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly parsed))
            {
                site.Date = parsed;
            }
            else
            {
                WriteLine($"WARNING: Could not read the date \"{date}\" in {site.MarkdownFilePath}. Expected ISO 8601, e.g. 2026-09-27. Using {site.Date:yyyy-MM-dd} instead.");
            }
        }

        if (TryRead("path", out string path))
        {
            site.Path = path;
        }

        if (TryRead("tags", out string tags))
        {
            site.Tags = tags
                .Split(',')
                .Select(tag => tag.Trim())
                .Where(tag => tag.Length > 0)
                .ToList();
        }

        // Absent key or empty value means "not given", so the default survives.
        bool TryRead(string key, out string value)
        {
            if (metaData.TryGetValue(key, out string? found) && !string.IsNullOrWhiteSpace(found))
            {
                value = found;
                return true;
            }

            value = string.Empty;
            return false;
        }
    }

    /// <summary>
    /// Whether the path names a markdown file. Case insensitive, so "NOTES.MD" counts too.
    /// </summary>
    private static bool IsMarkdownFile(string path)
        => Path.GetExtension(path).Equals(".md", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Whether the path names an index file. "index.md" is a reserved name: it carries the
    /// content of the section or page itself and never becomes an item of its own.
    /// Compared against the filename, not the end of the path, so that "my-index.md" is an
    /// ordinary item.
    /// </summary>
    private static bool IsIndexFile(string path)
        => Path.GetFileName(path).Equals("index.md", StringComparison.OrdinalIgnoreCase);
}
