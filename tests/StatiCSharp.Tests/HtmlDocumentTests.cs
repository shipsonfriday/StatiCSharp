using Xunit;

namespace StatiCSharp.Tests;

public class HtmlDocumentTests
{
    private static Website AWebsite() =>
        Website.Create(url: "https://example.com", name: "My Website").WithLanguage("de-DE");

    private static string Head(Item site, Website? website = null)
    {
        website ??= AWebsite();
        return HtmlDocument.Wrap(
            website,
            site,
            themeHead: string.Empty,
            parserHead: string.Empty,
            body: string.Empty);
    }

    [Fact]
    public void ADoubleQuoteInTheTitleDoesNotEscapeTheTitleTag()
    {
        string html = Head(new Item { Title = "The \"quoted\" post" });

        Assert.Contains("<title>The &quot;quoted&quot; post</title>", html, StringComparison.Ordinal);
    }

    [Fact]
    public void ADoubleQuoteInTheDescriptionDoesNotEndTheAttribute()
    {
        string html = Head(new Item { Description = "He said \"hi\" & left" });

        Assert.Contains(
            "<meta name=\"description\" content=\"He said &quot;hi&quot; &amp; left\">",
            html,
            StringComparison.Ordinal);
    }

    [Fact]
    public void ADoubleQuoteInTheAuthorDoesNotEndTheAttribute()
    {
        // Author came straight from the front matter with no encoding at all.
        string html = Head(new Item { Author = "Ro\"land" });

        Assert.Contains("<meta name=\"author\" content=\"Ro&quot;land\">", html, StringComparison.Ordinal);
    }

    [Fact]
    public void TagsAreEncodedInTheKeywords()
    {
        string html = Head(new Item { Tags = ["a&b", "c\"d"] });

        Assert.Contains(
            "<meta name=\"keywords\" content=\"a&amp;b, c&quot;d\">",
            html,
            StringComparison.Ordinal);
    }

    [Fact]
    public void TheLanguageEndsUpInTheHtmlTag()
    {
        string html = Head(new Item());

        Assert.Contains("<html lang=\"de-DE\">", html, StringComparison.Ordinal);
    }

    [Fact]
    public void TheHeadContentsAndTheBodyAreWrittenThroughUnchanged()
    {
        // Those two are html by the time they get here and must not be encoded, or every
        // page would render as visible source.
        Website website = AWebsite();
        string html = HtmlDocument.Wrap(
            website,
            new Item(),
            themeHead: "<link rel=\"stylesheet\" href=\"/theme.css\">",
            parserHead: "<script src=\"/parser.js\"></script>",
            body: "<body><p>Hello & welcome</p></body>");

        Assert.Contains("<link rel=\"stylesheet\" href=\"/theme.css\">", html, StringComparison.Ordinal);
        Assert.Contains("<script src=\"/parser.js\"></script>", html, StringComparison.Ordinal);
        Assert.Contains("<body><p>Hello & welcome</p></body>", html, StringComparison.Ordinal);
    }

    [Fact]
    public void RejectsNullArguments()
    {
        Website website = AWebsite();
        string empty = string.Empty;

        Assert.Throws<ArgumentNullException>(() => HtmlDocument.Wrap(null!, new Item(), empty, empty, empty));
        Assert.Throws<ArgumentNullException>(() => HtmlDocument.Wrap(website, null!, empty, empty, empty));
        Assert.Throws<ArgumentNullException>(() => HtmlDocument.Wrap(website, new Item(), null!, empty, empty));
        Assert.Throws<ArgumentNullException>(() => HtmlDocument.Wrap(website, new Item(), empty, null!, empty));
        Assert.Throws<ArgumentNullException>(() => HtmlDocument.Wrap(website, new Item(), empty, empty, null!));
    }
}
