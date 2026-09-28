using System.Collections.Generic;

namespace StatiCSharp.Interfaces
{
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
        /// A list of the items corresponding to this section.
        /// </summary>
        List<IItem> Items { get; }

        /// <summary>
        /// Adds an item to the section and sorts them by date.
        /// </summary>
        /// <param name="item">The item to add.</param>
        void AddItem(IItem item);
    }
}
