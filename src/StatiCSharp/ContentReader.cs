using StatiCSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using static StatiCSharp.StatiCSharpConsole;

namespace StatiCSharp;

/// <summary>
/// Reads the markdown files in the content directory and fills a website with what it finds:
/// the index, the pages, the sections and their items.
/// <para>
/// The counterpart of <see cref="OutputWriter"/>. The manager used to do both, which is why
/// reading and writing shared fields and the order of the two was a matter of reading
/// <c>MakeAsync</c> from top to bottom.
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
    /// Reads the content directory and puts the index, pages, sections and items into the
    /// given website. Anything the website held before is discarded.
    /// </summary>
    /// <param name="website">The website to fill.</param>
    /// <exception cref="ArgumentNullException"><paramref name="website"/> is null.</exception>
    internal void ReadInto(IWebsite website)
    {
        ArgumentNullException.ThrowIfNull(website);

        // Reading appends to the website, so a website used for a second run would end up
        // holding every section and page twice - and the index would keep the values of the
        // previous run if there is no index.md. Start from empty.
        website.Pages.Clear();
        website.Sections.Clear();
        Reset(website.Index);

        string[] directoriesOfContent = Directory.GetDirectories(_contentDirectory);

        // Index
        string pathOfIndex = Path.Combine(_contentDirectory, "index.md");
        if (File.Exists(pathOfIndex))
            ReadIndex(website, pathOfIndex);


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
                    ReadPage(website, file);
            }

            foreach (string directory in dirs)
            {
                ProcessAllPagesInDirectory(directory);
            }
        }


        // Sections
        foreach (string directory in directoriesOfContent)
        {
            string nameOfCurrentDirectory = Path.GetFileName(directory);
            if (website.MakeSectionsFor.Contains(nameOfCurrentDirectory))
            {
                string pathOfSectionIndexFile = Path.Combine(directory, "index.md");

                if (File.Exists(pathOfSectionIndexFile))
                    ReadSection(website, pathOfSectionIndexFile);
            }
        }
    }

    /// <summary>
    /// Reads the index of the website. The index object cannot be replaced - a caller may
    /// pass an <see cref="IWebsite"/> implementation of their own - so it is filled in place.
    /// </summary>
    private void ReadIndex(IWebsite website, string path)
    {
        FillFromMarkdown(website.Index, path);
    }

    /// <summary>
    /// Reads a page and appends it to the website.
    /// </summary>
    private void ReadPage(IWebsite website, string path)
    {
        Page page = new()
        {
            Hierarchy = HierarchyOf(path),
        };

        FillFromMarkdown(page, path);

        website.Pages.Add(page);
    }

    /// <summary>
    /// Reads a section from its index file, together with every item in the same directory,
    /// and appends it to the website.
    /// </summary>
    private void ReadSection(IWebsite website, string path)
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
                section.AddItem(ReadItem(itemFile, section.SectionName));
            }
        }

        website.Sections.Add(section);
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
        site.Content = _htmlBuilder.ToHtml(MarkdownFactory.ParseContent(path));
        site.MarkdownFileName = Path.GetFileName(path);
        site.MarkdownFilePath = path;

        MapMetaData(MarkdownFactory.ParseMetaData(path), site);
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
    /// Puts a site back into the state a freshly created one is in.
    /// <para>
    /// Needed because the website carries both the configuration the caller gave and the
    /// content read from disk. Separating those is the real fix; until then, reading starts
    /// by undoing what an earlier run left behind.
    /// </para>
    /// <para>
    /// Every settable property of <see cref="ISite"/> has to be listed here. A test walks
    /// the interface and fails if one is missing.
    /// </para>
    /// </summary>
    /// <param name="site">The site to reset.</param>
    private static void Reset(ISite site)
    {
        site.Title = string.Empty;
        site.Description = string.Empty;
        site.Author = string.Empty;
        site.Date = DateOnly.FromDateTime(DateTime.Now);
        site.DateLastModified = DateOnly.FromDateTime(DateTime.Now);
        site.Path = string.Empty;
        site.Tags = [];
        site.Content = string.Empty;
        site.MarkdownFileName = string.Empty;
        site.MarkdownFilePath = string.Empty;
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
