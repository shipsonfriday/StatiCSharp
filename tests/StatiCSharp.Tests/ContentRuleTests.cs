using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StatiCSharp.HtmlComponents;
using StatiCSharp.Interfaces;
using Xunit;

namespace StatiCSharp.Tests;

/// <summary>
/// Where the html standard says what may be inside an element, the types say it too.
/// <para>
/// What this guards cannot be written as a test, because the point is that it does not compile:
/// <c>new Ul(new Div("x"))</c>, <c>new Ul { "text" }</c>, <c>ul.Add(new Div("x"))</c> and
/// <c>new Input { new Div("x") }</c> are all errors now. So these tests check the shape of the
/// api that produces those errors, and that the allowed combinations still work.
/// </para>
/// </summary>
public class ContentRuleTests
{
    /// <summary>
    /// Every element with a content rule, and what it may hold.
    /// </summary>
    public static TheoryData<Type, Type> Restricted => new()
    {
        { typeof(Ul), typeof(Li) },
        { typeof(Ol), typeof(Li) },
        { typeof(Thead), typeof(Tr) },
        { typeof(Tbody), typeof(Tr) },
        { typeof(Tfoot), typeof(Tr) },
        { typeof(Tr), typeof(ITableCell) },
        { typeof(Select), typeof(Option) },
        { typeof(Br), typeof(NoContent) },
        { typeof(Hr), typeof(NoContent) },
        { typeof(Image), typeof(NoContent) },
        { typeof(Input), typeof(NoContent) },
        { typeof(Link), typeof(NoContent) },
        { typeof(Meta), typeof(NoContent) },
        { typeof(Source), typeof(NoContent) },
    };

    [Theory]
    [MemberData(nameof(Restricted))]
    public void AnElementWithARuleAcceptsNothingElse(Type element, Type child)
    {
        // One public Add, taking exactly what the standard allows. A second one taking
        // IHtmlComponent or string would quietly undo the whole thing, because a collection
        // initializer and a plain Add call would both find it again.
        MethodInfo[] adds = [.. element.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(method => method.Name == "Add")];

        MethodInfo add = Assert.Single(adds);

        Assert.Equal(child, Assert.Single(add.GetParameters()).ParameterType);
    }

    [Theory]
    [MemberData(nameof(Restricted))]
    public void AndNoConstructorTakesAnythingElse(Type element, Type child)
    {
        foreach (ConstructorInfo constructor in element.GetConstructors())
        {
            foreach (ParameterInfo parameter in constructor.GetParameters())
            {
                Type taken = parameter.ParameterType.IsArray
                    ? parameter.ParameterType.GetElementType()!
                    : parameter.ParameterType;

                // An attribute value, e.g. Image(string src), is not content.
                if (taken == typeof(string) && constructor.GetParameters().Length == 1
                    && !parameter.Name!.Equals("text", StringComparison.Ordinal))
                {
                    continue;
                }

                Assert.True(
                    taken == child,
                    $"{element.Name} has a constructor taking {taken.Name}, which is not {child.Name}");
            }
        }
    }

    [Fact]
    public void AnElementWithoutARuleStillTakesAnything()
    {
        // The other ~50 elements are untouched, and so is a custom element in a theme.
        Assert.Equal(
            "<div><h1>Title</h1>text<ul><li>a</li></ul></div>",
            new Div(new H1("Title"), new Text("text"), new Ul(new Li("a"))).Render());

        Assert.Equal("<div><p>x</p>text</div>", new Div { new Paragraph("x"), "text" }.Render());
    }

    [Fact]
    public void TheAllowedCombinationsStillWork()
    {
        Assert.Equal("<ul><li>a</li><li>b</li></ul>", new Ul(new Li("a"), new Li("b")).Render());
        Assert.Equal("<ul><li>a</li></ul>", new Ul { new Li("a") }.Render());
        Assert.Equal("<ol><li>a</li></ol>", new Ol(new Li("a")).Render());
        Assert.Equal("<tr><th>H</th><td>C</td></tr>", new Tr(new Th("H"), new Td("C")).Render());
        Assert.Equal("<tbody><tr><td>C</td></tr></tbody>", new Tbody(new Tr(new Td("C"))).Render());
        Assert.Equal("<select><option>A</option></select>", new Select(new Option("A")).Render());
    }

    [Fact]
    public void AVoidElementCanStillBeGivenItsAttributes()
    {
        // Only content is refused, not the element's own attributes.
        Assert.Equal("<input type=\"checkbox\" checked>", new Input().Type(InputType.Checkbox).Attribute("checked").Render());
        Assert.Equal("<img src=\"/me.png\" alt=\"Me\">", new Image("/me.png").Alt("Me").Render());
    }

    [Fact]
    public void NoContentCannotBeMade()
    {
        // Which is what makes a void element hold nothing: Add(NoContent) has no argument it
        // could ever be given.
        Assert.Empty(typeof(NoContent).GetConstructors(BindingFlags.Public | BindingFlags.Instance));
    }

    [Fact]
    public void AFullTableReads()
    {
        string table = new Table(
                new Caption("Sales"),
                new Thead(new Tr(new Th("Month"), new Th("Total"))),
                new Tbody(
                    new Tr(new Td("January"), new Td("12")),
                    new Tr(new Td("February"), new Td("15"))))
            .Render();

        Assert.Equal(
            "<table><caption>Sales</caption><thead><tr><th>Month</th><th>Total</th></tr></thead>"
            + "<tbody><tr><td>January</td><td>12</td></tr><tr><td>February</td><td>15</td></tr></tbody></table>",
            table);
    }
}
