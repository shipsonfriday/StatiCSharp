using StatiCSharp.Interfaces;
using StatiCSharp.Tools;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using static StatiCSharp.StatiCSharpConsole;

namespace StatiCSharp;

public partial class WebsiteManager : IWebsiteManager
{
    /// <summary>
    /// Asynchronously creates and writes the tags pages of the website.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous tags generating operation.</returns>
    private async Task MakeTagListsAsync(OutputWriter output)
    {
        // Collect all available tags
        List<string> tags = new List<string>();
        foreach (ISection currentSection in Website.Sections)
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
            WriteLine($"WARNING: The tags {string.Join(", ", collision.Select(tag => $"\"{tag}\""))} all lead to /tag/{collision.Key}. Only one of them will be written.");
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
                WriteLine($"WARNING: The tag \"{tag}\" has no characters that can be used in a url. Skipping it.");
                return;
            }

            List<IItem> itemsWithCurrentTag = new();
            // Collect all items with the current tag
            foreach (ISection currentSection in Website.Sections)
            {
                foreach (IItem item in currentSection.Items)
                {
                    if (item.Tags.Contains(tag))
                    {
                        itemsWithCurrentTag.Add(item);
                    }
                }
            }

            // Write tags sites to files
            Item tagPage = new();
            tagPage.Title = $"{tag} | {Website.Name}";
            string body = HtmlFactory.MakeTagListHtml(itemsWithCurrentTag, tag);
            string head = HtmlFactory.MakeHeadHtml();
            string page = HtmlDocument.Wrap(Website, tagPage, head, _htmlBuilder.AdditionalHeaderContent, body);

            // Create directory, if it does not excist
            string path = Directory.CreateDirectory(Path.Combine(Output, "tag", slug)).ToString();

            if (!output.ClaimPath(path))
            {
                WriteLine($"WARNING: The path {path} is already in use. Change the path in meta data to avoid duplicates.");
            }

            await output.WriteAsync(path, "index.html", page);

        }
    }
}
