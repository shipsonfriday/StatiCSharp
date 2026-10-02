using System;
using System.Collections.Generic;
using System.Linq;
using StatiCSharp.HtmlComponents;
using Xunit;

namespace StatiCSharp.Tests;

/// <summary>
/// The attribute values the html standard fixes are typed, so a theme cannot mistype them.
/// </summary>
public class AttributeValueTests
{
    [Theory]
    [InlineData(InputType.Text, "text")]
    [InlineData(InputType.Checkbox, "checkbox")]
    [InlineData(InputType.DateTimeLocal, "datetime-local")]
    [InlineData(InputType.Email, "email")]
    public void AnInputTypeIsWrittenTheWayTheStandardSpellsIt(InputType type, string expected)
    {
        Assert.Equal($"<input type=\"{expected}\">", new Input().Type(type).Render());
    }

    [Theory]
    [InlineData(LinkTarget.Blank, "_blank")]
    [InlineData(LinkTarget.Self, "_self")]
    [InlineData(LinkTarget.Parent, "_parent")]
    [InlineData(LinkTarget.Top, "_top")]
    public void ALinkTargetKeepsItsUnderscore(LinkTarget target, string expected)
    {
        Assert.Contains($"target=\"{expected}\"", new A("x").Target(target).Render(), StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(LinkRelation.Stylesheet, "stylesheet")]
    [InlineData(LinkRelation.DnsPrefetch, "dns-prefetch")]
    [InlineData(LinkRelation.ModulePreload, "modulepreload")]
    [InlineData(LinkRelation.Canonical, "canonical")]
    public void ALinkRelationIsWrittenTheWayTheStandardSpellsIt(LinkRelation relation, string expected)
    {
        Assert.Equal($"<link rel=\"{expected}\">", new Link().Rel(relation).Render());
    }

    [Theory]
    [InlineData(FormMethod.Get, "get")]
    [InlineData(FormMethod.Post, "post")]
    [InlineData(FormMethod.Dialog, "dialog")]
    public void AFormMethodIsLowercase(FormMethod method, string expected)
    {
        Assert.Contains($"method=\"{expected}\"", new Form().Method(method).Render(), StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(ButtonType.Submit, "submit")]
    [InlineData(ButtonType.Button, "button")]
    public void AButtonTypeIsLowercase(ButtonType type, string expected)
    {
        Assert.Contains($"type=\"{expected}\"", new Button("Go").Type(type).Render(), StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(CellScope.Row, "row")]
    [InlineData(CellScope.RowGroup, "rowgroup")]
    [InlineData(CellScope.ColGroup, "colgroup")]
    public void ACellScopeIsOneWordAndLowercase(CellScope scope, string expected)
    {
        Assert.Contains($"scope=\"{expected}\"", new Th("Name").Scope(scope).Render(), StringComparison.Ordinal);
    }

    [Fact]
    public void EveryMemberOfEveryEnumHasAValue()
    {
        // The values are written out by hand, so a member added without one would fall into the
        // arm that throws. This walks all of them rather than trusting the cases above.
        Dictionary<Type, Func<object, string>> mappings = new()
        {
            [typeof(InputType)] = value => AttributeValueOf((InputType)value),
            [typeof(LinkTarget)] = value => AttributeValueOf((LinkTarget)value),
            [typeof(LinkRelation)] = value => AttributeValueOf((LinkRelation)value),
            [typeof(FormMethod)] = value => AttributeValueOf((FormMethod)value),
            [typeof(ButtonType)] = value => AttributeValueOf((ButtonType)value),
            [typeof(CellScope)] = value => AttributeValueOf((CellScope)value),
        };

        foreach ((Type type, Func<object, string> mapping) in mappings)
        {
            foreach (object member in Enum.GetValues(type))
            {
                string written = mapping(member);

                Assert.False(string.IsNullOrWhiteSpace(written), $"{type.Name}.{member} has no value");
                Assert.Equal(written.Trim(), written);
                Assert.DoesNotContain(' ', written);
            }
        }

        static string AttributeValueOf<T>(T value) where T : struct, Enum => value switch
        {
            InputType type => AttributeValue.Of(type),
            LinkTarget target => AttributeValue.Of(target),
            LinkRelation relation => AttributeValue.Of(relation),
            FormMethod method => AttributeValue.Of(method),
            ButtonType type => AttributeValue.Of(type),
            CellScope scope => AttributeValue.Of(scope),
            _ => throw new NotSupportedException(typeof(T).Name),
        };
    }

    [Fact]
    public void EveryEnumCoversWhatTheStandardLists()
    {
        // A reminder of the sizes, so adding a member to the standard's list without adding it
        // here is noticed. Update the number together with the enum.
        Assert.Equal(22, Enum.GetValues<InputType>().Length);
        Assert.Equal(4, Enum.GetValues<LinkTarget>().Length);
        Assert.Equal(17, Enum.GetValues<LinkRelation>().Length);
        Assert.Equal(3, Enum.GetValues<FormMethod>().Length);
        Assert.Equal(3, Enum.GetValues<ButtonType>().Length);
        Assert.Equal(4, Enum.GetValues<CellScope>().Length);
    }

    [Fact]
    public void ATargetOfYourOwnIsNoLongerDroppedWithoutAWord()
    {
        // It used to check the value against the four keywords and silently write no attribute
        // at all when it did not match, so a mistyped "_blnak" produced a link with no target.
        Assert.Contains("target=\"myframe\"", new A("x").Target("myframe").Render(), StringComparison.Ordinal);
        Assert.Contains("target=\"_blnak\"", new A("x").Target("_blnak").Render(), StringComparison.Ordinal);
    }
}
