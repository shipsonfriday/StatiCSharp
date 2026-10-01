using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using StatiCSharp.Interfaces;
using Xunit;

namespace StatiCSharp.Tests;

/// <summary>
/// Covers running the generator more than once, which used to leave the website holding
/// every section and page once per run.
/// </summary>
public class RepeatedRunTests
{
    private static Website WriteSource(TempDirectory directory)
    {
        directory.WriteFile("Content/index.md", "---", "Title: Home", "---", "Welcome.");
        directory.WriteFile("Content/posts/index.md", "---", "Title: Posts", "---", "The section.");
        directory.WriteFile("Content/posts/a-post.md", "---", "Title: A Post", "Date: 2026-01-01", "Tags: one", "---", "Body.");
        directory.WriteFile("Content/about/index.md", "---", "Title: About", "---", "About.");
        Directory.CreateDirectory(Path.Combine(directory.Path, "Resources"));

        return Website.Create(url: "https://example.com", name: "My Website").WithSections("posts");
    }

    [Fact]
    public async Task ReusingTheWebsiteDoesNotDuplicateItsContent()
    {
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);

        for (int run = 1; run <= 3; run++)
        {
            await WebsiteManager.For(website, directory.Path).MakeAsync();

            Assert.Single(website.Sections);
            Assert.Single(website.Pages);
            Assert.Single(Assert.Single(website.Sections).Items);
        }
    }

    [Fact]
    public async Task ReusingTheManagerDoesNotDuplicateItsContentEither()
    {
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);
        WebsiteManager manager = WebsiteManager.For(website, directory.Path);

        await manager.MakeAsync();
        await manager.MakeAsync();

        Assert.Single(website.Sections);
        Assert.Single(website.Pages);
    }

    [Fact]
    public async Task TheIndexDoesNotKeepValuesFromAnEarlierRun()
    {
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);

        await WebsiteManager.For(website, directory.Path).MakeAsync();
        Assert.Equal("Home", website.Index.Title);

        // Second run without an index.md: the index has to be empty again, not still "Home".
        File.Delete(Path.Combine(directory.Path, "Content", "index.md"));
        await WebsiteManager.For(website, directory.Path).MakeAsync();

        Assert.Equal(string.Empty, website.Index.Title);
        Assert.Equal(string.Empty, website.Index.Content);
    }

    [Fact]
    public async Task TheOutputIsTheSameAfterASecondRun()
    {
        using var directory = new TempDirectory();
        Website website = WriteSource(directory);
        string output = Path.Combine(directory.Path, "Output");

        await WebsiteManager.For(website, directory.Path).MakeAsync();
        string[] first = [.. Directory.GetFiles(output, "*", SearchOption.AllDirectories).Order()];

        await WebsiteManager.For(website, directory.Path).MakeAsync();
        string[] second = [.. Directory.GetFiles(output, "*", SearchOption.AllDirectories).Order()];

        Assert.Equal(first, second);
    }

    [Fact]
    public void EverySettablePropertyOfASiteIsReset()
    {
        // The reset lists ISite's properties by hand. If one is added without extending it,
        // a reused website would silently carry that value into the next run.
        var site = new Item
        {
            Title = "t",
            Description = "d",
            Author = "a",
            Date = new DateOnly(2000, 1, 1),
            DateLastModified = new DateOnly(2000, 1, 1),
            Path = "p",
            Tags = ["x"],
            Content = "c",
            MarkdownFileName = "f",
            MarkdownFilePath = "fp",
        };

        typeof(WebsiteManager)
            .GetMethod("Reset", BindingFlags.NonPublic | BindingFlags.Static)!
            .Invoke(null, [site]);

        string[] stillSet = [.. typeof(ISite)
            .GetProperties()
            .Where(property => property.CanWrite)
            .Where(property => !IsDefault(property.GetValue(site)))
            .Select(property => property.Name)
            .Order()];

        Assert.Empty(stillSet);

        static bool IsDefault(object? value) => value switch
        {
            string text => text.Length == 0,
            DateOnly date => date == DateOnly.FromDateTime(DateTime.Now),
            System.Collections.ICollection collection => collection.Count == 0,
            _ => false,
        };
    }
}
