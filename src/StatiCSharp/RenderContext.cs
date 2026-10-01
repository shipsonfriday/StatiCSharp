using StatiCSharp.Interfaces;
using System.Collections.Generic;

namespace StatiCSharp;

/// <summary>
/// Everything a theme may look at while it renders one site: the website's configuration and
/// the whole content that was read.
/// <para>
/// A theme used to be handed the website in its constructor - before anything was read, so the
/// content it saw was empty at that moment and filled in later. That only worked because the
/// collections were the very same objects the reader kept writing into. A theme gets the data
/// at render time now and holds nothing of its own, which makes it reusable and testable.
/// </para>
/// <para>
/// Only StatiC# creates one of these during a run, but the properties are settable at
/// construction so that a theme's own tests can build one:
/// <code>
/// var context = new RenderContext { Website = Website.Create("https://example.com", "Test") };
/// </code>
/// Leaving one out gives an empty index, no pages and no sections, which is a valid website.
/// New properties can be added here without breaking a theme.
/// </para>
/// </summary>
public sealed class RenderContext
{
    /// <summary>
    /// The configuration of the website: its url, name, description and language.
    /// </summary>
    public required IWebsite Website { get; init; }

    /// <summary>
    /// The index (homepage) of the website.
    /// </summary>
    public IIndex Index { get; init; } = new Index();

    /// <summary>
    /// Every page of the website, in the order they were read.
    /// </summary>
    public IReadOnlyList<IPage> Pages { get; init; } = [];

    /// <summary>
    /// Every section of the website with its items, in the order they were read.
    /// </summary>
    public IReadOnlyList<ISection> Sections { get; init; } = [];
}
