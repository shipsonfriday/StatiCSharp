namespace StatiCSharp.Interfaces;

/// <summary>
/// The interface a section of the website must implement.
/// </summary>
public interface ISection: ISite
{
    /// <summary>
    /// Name if the section. In the url the equivalent to `hierachy` of a page. (i.e. "dev" for rolandbraun.com/dev)
    /// </summary>
    string SectionName { get; set; }

    /// <summary>
    /// The items corresponding to this section, newest first.
    /// <para>
    /// Read only, so that rendering cannot change what it is rendering. It used to be a
    /// <see cref="List{T}"/>, and the default theme sorted it in place while building a page -
    /// a section came out of a render in a different order than it went in.
    /// <see cref="AddItem"/> is the way to add one.
    /// </para>
    /// </summary>
    IReadOnlyList<IItem> Items { get; }

    /// <summary>
    /// Adds an item to the section, keeping the items ordered by date, newest first.
    /// </summary>
    /// <param name="item">The item to add.</param>
    void AddItem(IItem item);
}
