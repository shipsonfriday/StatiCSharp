namespace StatiCSharp.Interfaces
{
    /// <summary>
    /// The interface a page of the website must implement.
    /// </summary>
    public interface IPage: ISite
    {
        /// Hierachy of the page. (i.e. "dev" for rolandbraun.com/dev)
        string Hierarchy { get; set; }
    }
}
