using System;
using System.Linq;
using Xunit;

namespace StatiCSharp.Tests;

public class SectionTests
{
    private static Item AnItem(string title, string date) =>
        new() { Title = title, Date = DateOnly.Parse(date, System.Globalization.CultureInfo.InvariantCulture) };

    [Fact]
    public void AddItem_KeepsTheItemsNewestFirst()
    {
        // The sorting used to be a discarded OrderByDescending call, so the items stayed
        // in whatever order they were added.
        var section = new Section { SectionName = "posts" };

        section.AddItem(AnItem("oldest", "2024-01-01"));
        section.AddItem(AnItem("newest", "2026-01-01"));
        section.AddItem(AnItem("middle", "2025-01-01"));

        Assert.Equal(["newest", "middle", "oldest"], section.Items.Select(item => item.Title));
    }

    [Fact]
    public void AddItem_KeepsInsertionOrderForItemsSharingADate()
    {
        // Stability matters: if items with the same date could swap places between runs,
        // GitMode would rewrite files whose content did not actually change.
        var section = new Section { SectionName = "posts" };

        section.AddItem(AnItem("first", "2025-06-01"));
        section.AddItem(AnItem("second", "2025-06-01"));
        section.AddItem(AnItem("third", "2025-06-01"));

        Assert.Equal(["first", "second", "third"], section.Items.Select(item => item.Title));
    }

    [Fact]
    public void AddItem_PlacesAnOlderItemAfterOnesSharingALaterDate()
    {
        var section = new Section { SectionName = "posts" };

        section.AddItem(AnItem("a", "2025-06-01"));
        section.AddItem(AnItem("b", "2025-06-01"));
        section.AddItem(AnItem("older", "2024-01-01"));
        section.AddItem(AnItem("c", "2025-06-01"));

        Assert.Equal(["a", "b", "c", "older"], section.Items.Select(item => item.Title));
    }

    [Fact]
    public void AddItem_RejectsNull()
    {
        var section = new Section { SectionName = "posts" };

        Assert.Throws<ArgumentNullException>(() => section.AddItem(null!));
    }

    [Fact]
    public void Url_IsTheSectionName()
    {
        var section = new Section { SectionName = "posts" };

        Assert.Equal("/posts", section.Url);
    }
}
