namespace StatiCSharp.Interfaces
{
    /// <summary>
    /// The interface an item of the website must implement.
    /// </summary>
    public  interface IItem: ISite
    {
        /// <summary>
        /// The section the items is a part of.
        /// </summary>
        string Section { get; set; }
    }
}
