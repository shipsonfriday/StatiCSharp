using StatiCSharp.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace StatiCSharp;

public partial class WebsiteManager : IWebsiteManager
{
    /// <summary>
    /// Asynchronous generates index, pages, sections and items for the IWebsite object from the markdown files in the Content directory.
    /// </summary>
    /// <returns></returns>
    internal async Task GenerateSitesFromMarkdownAsync()
    {
        // Reading appends to the website, so a website used for a second run would end up
        // holding every section and page twice - and the index would keep the values of the
        // previous run if there is no index.md. Start from empty.
        Website.Pages.Clear();
        Website.Sections.Clear();
        Reset(Website.Index);

        string[] directoriesOfContent = Directory.GetDirectories(Content);

        // Index
        string pathOfIndex = Path.Combine(Content, "index.md");
        if (File.Exists(pathOfIndex))
            LoadSiteFromMarkdown<IIndex>(pathOfIndex);


        // Pages
        foreach (string directory in directoriesOfContent)
        {
            string nameOfCurrentDirectory = Path.GetFileName(directory);
            if (!Website.MakeSectionsFor.Contains(nameOfCurrentDirectory))
            {
                await ProcessAllPagesInDirectory(directory);
            }
        }

        async Task ProcessAllPagesInDirectory(string dir)
        {
            string[] files = Directory.GetFiles(dir);
            var dirs = Directory.GetDirectories(dir);

            foreach (string file in files)
            {
                if (IsMarkdownFile(file))
                    LoadSiteFromMarkdown<IPage>(file);
            }

            foreach (string directory in dirs)
            {
                await ProcessAllPagesInDirectory(directory);
            }
        }


        // Sections
        foreach (string directory in directoriesOfContent)
        {
            string nameOfCurrentDirectory = Path.GetFileName(directory);
            if (Website.MakeSectionsFor.Contains(nameOfCurrentDirectory))
            {
                string pathOfSectionIndexFile = Path.Combine(directory, "index.md");

                if (File.Exists(pathOfSectionIndexFile))
                    LoadSiteFromMarkdown<ISection>(pathOfSectionIndexFile);
            }
        }
    }

    private void LoadSiteFromMarkdown<T>(string path)
    {
        var metaData = MarkdownFactory.ParseMetaData(path);
        var content = MarkdownFactory.ParseContent(path);
        var contentAsHtml = _htmlBuilder.ToHtml(content);
        var filename = Path.GetFileName(path);

        if (typeof(T) == typeof(IIndex))
        {
            Website.Index.Content = contentAsHtml;
            Website.Index.MarkdownFileName = filename;
            Website.Index.MarkdownFilePath = path;
            MapMetaData(metaData, Website.Index);
            return;
        }

        if (typeof(T) == typeof(IPage))
        {
            Page currentPage = new();
            currentPage.Content = contentAsHtml;
            currentPage.MarkdownFileName = filename;
            currentPage.MarkdownFilePath = path;
            MapMetaData(metaData, currentPage);

            string hierarchy = path.Remove(0, Content.Length);
            if (hierarchy[0] == '/' || hierarchy[0] == '\\')
            {
                hierarchy = hierarchy.Remove(0, 1);
            }

            hierarchy = Path.GetDirectoryName(hierarchy)!;

            currentPage.Hierarchy = hierarchy;

            Website.Pages.Add(currentPage);
            return;
        }

        if (typeof(T) == typeof(ISection))
        {
            Section currentSection = new();
            string currentSectionName = Path.GetDirectoryName(path)!;
            currentSectionName = Path.GetFileName(currentSectionName);
            currentSection.SectionName = currentSectionName;
            currentSection.Content = contentAsHtml;
            currentSection.MarkdownFileName = filename;
            currentSection.MarkdownFilePath = path;
            MapMetaData(metaData, currentSection);

            string sectionFolder = Path.GetDirectoryName(path)!;
            string[] itemFiles = Directory.GetFiles(sectionFolder);

            foreach (string itemFile in itemFiles)
            {
                if (IsMarkdownFile(itemFile) && !IsIndexFile(itemFile))
                {
                    var itemMetaData = MarkdownFactory.ParseMetaData(itemFile);
                    var itemContent = MarkdownFactory.ParseContent(itemFile);
                    var itemContentAsHtml = _htmlBuilder.ToHtml(itemContent);
                    var itemLastModified = DateOnly.FromDateTime(File.GetLastWriteTime(itemFile));

                    IItem currentItem = new Item
                    {
                        Content = itemContentAsHtml,
                        MarkdownFileName = Path.GetFileName(itemFile),
                        MarkdownFilePath = itemFile,
                        Section = currentSectionName,
                        DateLastModified = itemLastModified,

                        // Fallback for items whose meta data carries no date. Set before
                        // mapping, because MapMetaData leaves the value in place when no
                        // date is given - no need to route the date through the
                        // dictionary as a string, which also made it culture dependent.
                        Date = itemLastModified,
                    };

                    MapMetaData(itemMetaData, currentItem);

                    currentSection.AddItem(currentItem);
                }
            }
            Website.Sections.Add(currentSection);
            return;
        }

        throw new NotImplementedException(message:$"The given type-parameter {typeof(T)} is not supported by this method.");

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
