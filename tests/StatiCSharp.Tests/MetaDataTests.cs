using System.Globalization;
using Xunit;

namespace StatiCSharp.Tests;

public class MetaDataTests
{
    private static Item AnItem() => new() { MarkdownFilePath = "/content/posts/a-post.md" };

    [Fact]
    public void MapMetaData_ReadsEveryKnownKey()
    {
        Item site = AnItem();
        var metaData = new Dictionary<string, string>
        {
            ["title"] = "My Post",
            ["description"] = "A short description",
            ["author"] = "Roland",
            ["date"] = "2026-09-27",
            ["path"] = "a-custom-path",
            ["tags"] = "one, two",
        };

        ContentReader.MapMetaData(metaData, site);

        Assert.Equal("My Post", site.Title);
        Assert.Equal("A short description", site.Description);
        Assert.Equal("Roland", site.Author);
        Assert.Equal(new DateOnly(2026, 9, 27), site.Date);
        Assert.Equal("a-custom-path", site.Path);
        Assert.Equal(["one", "two"], site.Tags);
    }

    [Fact]
    public void MapMetaData_LeavesDefaultsAloneWhenKeysAreMissing()
    {
        Item site = AnItem();
        DateOnly defaultDate = site.Date;

        ContentReader.MapMetaData([], site);

        Assert.Equal(string.Empty, site.Title);
        Assert.Equal(string.Empty, site.Author);
        Assert.Equal(defaultDate, site.Date);
        Assert.Empty(site.Tags);
    }

    [Fact]
    public void MapMetaData_TreatsAnEmptyValueAsNotGiven()
    {
        Item site = AnItem();
        DateOnly defaultDate = site.Date;

        ContentReader.MapMetaData(new Dictionary<string, string>
        {
            ["author"] = string.Empty,
            ["date"] = "   ",
            ["path"] = string.Empty,
        }, site);

        Assert.Equal(string.Empty, site.Author);
        Assert.Equal(defaultDate, site.Date);
        Assert.Equal(string.Empty, site.Path);
    }

    [Fact]
    public void MapMetaData_DoesNotProduceAnEmptyTagFromAnEmptyTagsLine()
    {
        // "".Split(',') yields one empty entry, which used to become a tag with no name
        // and a /tag/ directory in the output.
        Item site = AnItem();

        ContentReader.MapMetaData(new Dictionary<string, string> { ["tags"] = string.Empty }, site);

        Assert.Empty(site.Tags);
    }

    [Fact]
    public void MapMetaData_DropsEmptyTagsAndTrimsTheRest()
    {
        Item site = AnItem();

        ContentReader.MapMetaData(new Dictionary<string, string> { ["tags"] = "one, , two," }, site);

        Assert.Equal(["one", "two"], site.Tags);
    }

    [Fact]
    public void MapMetaData_KeepsSpacesInsideATagName()
    {
        // The old code removed every space, turning "web dev" into "webdev".
        Item site = AnItem();

        ContentReader.MapMetaData(new Dictionary<string, string> { ["tags"] = "web dev, c#" }, site);

        Assert.Equal(["web dev", "c#"], site.Tags);
    }

    [Fact]
    public void MapMetaData_KeepsTheDefaultDateWhenTheValueIsUnusable()
    {
        // Used to be swallowed by an empty catch block, indistinguishable from a
        // missing key.
        Item site = AnItem();
        DateOnly defaultDate = site.Date;

        ContentReader.MapMetaData(new Dictionary<string, string> { ["date"] = "not a date" }, site);

        Assert.Equal(defaultDate, site.Date);
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("de-DE")]
    [InlineData("tr-TR")]
    public void MapMetaData_ReadsAnIsoDateInEveryCulture(string culture)
    {
        // The documented format is ISO 8601, so the same file has to yield the same date
        // no matter which machine runs the generator.
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo(culture);
            Item site = AnItem();

            ContentReader.MapMetaData(new Dictionary<string, string> { ["date"] = "2026-09-27" }, site);

            Assert.Equal(new DateOnly(2026, 9, 27), site.Date);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void MapMetaData_RejectsNullArguments()
    {
        Assert.Throws<ArgumentNullException>(() => ContentReader.MapMetaData(null!, AnItem()));
        Assert.Throws<ArgumentNullException>(() => ContentReader.MapMetaData([], null!));
    }

    [Fact]
    public void MapMetaData_TakesTheTitleAsPlainText()
    {
        // The title used to be run through Markdig and have its <p> tags cut off with a
        // string replace, so it arrived here as html. It is text now; the render sites
        // encode it.
        Item site = AnItem();

        ContentReader.MapMetaData(new Dictionary<string, string>
        {
            ["title"] = "My *great* post",
            ["description"] = "Uses <angle> brackets & an ampersand",
        }, site);

        Assert.Equal("My *great* post", site.Title);
        Assert.Equal("Uses <angle> brackets & an ampersand", site.Description);
    }

    [Fact]
    public void MapMetaData_KeepsQuotesInTheTitle()
    {
        Item site = AnItem();

        ContentReader.MapMetaData(new Dictionary<string, string> { ["title"] = "The \"quoted\" post" }, site);

        Assert.Equal("The \"quoted\" post", site.Title);
    }
}
