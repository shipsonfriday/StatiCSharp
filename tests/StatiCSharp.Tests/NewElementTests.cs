using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StatiCSharp.HtmlComponents;
using StatiCSharp.Interfaces;
using Xunit;
using static StatiCSharp.HtmlComponents.Tags;

namespace StatiCSharp.Tests;

/// <summary>
/// Covers the elements added beyond the original set, and the rule that holds them
/// together: a type's lowercased name is the tag it renders.
/// </summary>
public class NewElementTests
{
    private static IEnumerable<Type> AllElements()
        => typeof(Div).Assembly.GetTypes()
            .Where(type => type.Namespace == "StatiCSharp.HtmlComponents")
            .Where(type => type is { IsClass: true, IsAbstract: false, IsPublic: true })
            .Where(type => typeof(IHtmlComponent).IsAssignableFrom(type))
            .Where(type => type != typeof(Text))
            .Where(type => type.GetConstructor(Type.EmptyTypes) is not null);

    [Fact]
    public void EveryElementRendersATagMatchingItsLowercasedName()
    {
        // This is what makes short forms unnecessary. Paragraph and Image are the only two
        // exceptions and they already have P and Img; anything else added later has to
        // either follow the rule or bring its own short form.
        List<string> offenders = [];

        foreach (Type type in AllElements())
        {
            var element = (IHtmlComponent)Activator.CreateInstance(type)!;
            string expected = $"<{type.Name.ToLowerInvariant()}";

            if (!element.Render().StartsWith(expected, StringComparison.Ordinal))
            {
                offenders.Add($"{type.Name} renders {element.Render()}");
            }
        }

        Assert.Equal(["Image renders <img>", "Paragraph renders <p></p>"], offenders.Order());
    }

    [Fact]
    public void ThereAreFactoriesForEveryElement()
    {
        string[] withoutFactory = [.. AllElements()
            .Select(type => type.Name)
            .Where(name => typeof(Tags).GetMethod(name, BindingFlags.Public | BindingFlags.Static, [])
                is null)
            .Order()];

        Assert.Empty(withoutFactory);
    }

    [Theory]
    [InlineData("Link")]
    [InlineData("Meta")]
    [InlineData("Br")]
    [InlineData("Hr")]
    [InlineData("Source")]
    public void TheNewVoidElementsHaveNoClosingTag(string typeName)
    {
        Type type = typeof(Div).Assembly.GetType($"StatiCSharp.HtmlComponents.{typeName}")!;
        var element = (IHtmlComponent)Activator.CreateInstance(type)!;

        Assert.DoesNotContain("</", element.Render(), StringComparison.Ordinal);
    }

    [Fact]
    public void TheHeadElementsCanBeBuiltWithoutHandwrittenHtml()
    {
        // MakeHeadHtml used to return a string literal because there was no Link element.
        Assert.Equal(
            "<link rel=\"stylesheet\" href=\"/theme/styles.css\">",
            new Link().Rel("stylesheet").Href("/theme/styles.css").Render());

        Assert.Equal(
            "<meta name=\"description\" content=\"A short text\">",
            new Meta().Name("description").Content("A short text").Render());
    }

    [Fact]
    public void ATableReadsLikeTheMarkup()
    {
        string html = Table(
                Caption("Numbers"),
                Thead(Tr(Th("Name").Scope("col"), Th("Count").Scope("col"))),
                Tbody(Tr(Td("posts"), Td("42").ColSpan(2))))
            .Class("data")
            .Render();

        Assert.Equal(
            "<table class=\"data\"><caption>Numbers</caption>"
            + "<thead><tr><th scope=\"col\">Name</th><th scope=\"col\">Count</th></tr></thead>"
            + "<tbody><tr><td>posts</td><td colspan=\"2\">42</td></tr></tbody></table>",
            html);
    }

    [Fact]
    public void AFigureWithACaptionWorks()
    {
        string html = Figure(
                Img("/photo.png").Alt("A photo"),
                Figcaption("Taken last year"))
            .Render();

        Assert.Equal(
            "<figure><img src=\"/photo.png\" alt=\"A photo\">"
            + "<figcaption>Taken last year</figcaption></figure>",
            html);
    }

    [Fact]
    public void DisclosureAndTimeCarryTheirOwnAttributes()
    {
        Assert.Equal(
            "<details open><summary>More</summary><p>Text</p></details>",
            Details(Summary("More"), P("Text")).Open().Render());

        Assert.Equal(
            "<time datetime=\"2026-03-04\">March 04, 2026</time>",
            Time("March 04, 2026").DateTime("2026-03-04").Render());
    }

    [Fact]
    public void FlagAttributesRenderAsBareNames()
    {
        Assert.Equal("<script src=\"/app.js\" defer></script>", Script().Src("/app.js").Defer().Render());
        Assert.Equal("<video src=\"/v.mp4\" controls loop></video>", Video().Src("/v.mp4").Controls().Loop().Render());
    }

    [Fact]
    public void NumericAttributesAreFormattedInvariantly()
    {
        Assert.Equal("<td colspan=\"3\">x</td>", Td("x").ColSpan(3).Render());
        Assert.Equal("<textarea rows=\"5\" cols=\"40\"></textarea>", Textarea().Rows(5).Cols(40).Render());
    }
}
