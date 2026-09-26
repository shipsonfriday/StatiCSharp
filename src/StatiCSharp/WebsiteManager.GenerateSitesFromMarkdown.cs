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
    private async Task GenerateSitesFromMarkdownAsync()
    {
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
                if (file.EndsWith(".md"))
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
            IPage currentPage = new Page();
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
            ISection currentSection = new Section();
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
                if (!itemFile.EndsWith("index.md"))
                {
                    IItem currentItem = new Item();
                    var itemMetaData = MarkdownFactory.ParseMetaData(itemFile);
                    var itemContent = MarkdownFactory.ParseContent(itemFile);
                    var itemContentAsHtml = _htmlBuilder.ToHtml(itemContent);
                    var itemLastModified = DateOnly.FromDateTime(Directory.GetLastWriteTime(itemFile));

                    if (itemMetaData.ContainsKey("date"))
                    {
                        if (itemMetaData["date"] == string.Empty)
                        {
                            itemMetaData["date"] = currentItem.DateLastModified.ToString();
                        }
                    }
                    else
                    {
                        itemMetaData.Add("date", currentItem.DateLastModified.ToString());
                    }

                    currentItem.Content = itemContentAsHtml;
                    currentItem.MarkdownFileName = Path.GetFileName(itemFile);
                    currentItem.MarkdownFilePath = itemFile;
                    currentItem.Section = currentSectionName;
                    currentItem.DateLastModified = itemLastModified;
                    MapMetaData(itemMetaData, currentItem);

                    currentSection.AddItem(currentItem);
                }
            }
            Website.Sections.Add(currentSection);
            return;
        }

        throw new NotImplementedException(message:$"The given type-parameter {typeof(T)} is not supported by this method.");

    }
}
