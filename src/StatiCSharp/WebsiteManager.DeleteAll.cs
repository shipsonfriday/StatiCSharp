using StatiCSharp.Interfaces;
using System.IO;

namespace StatiCSharp;

public partial class WebsiteManager : IWebsiteManager
{
    /// <summary>
    /// Deletes all directories and files within the given path, without deleting the path folder itself.
    /// </summary>
    /// <param name="path"></param>
    /// <returns>void</returns>
    private void DeleteAll(string path)
    {
        DirectoryInfo directory = new DirectoryInfo(path);

        foreach (FileInfo file in directory.GetFiles())
        {
            file.Delete();
        }

        foreach (DirectoryInfo dir in directory.GetDirectories())
        {
            dir.Delete(true);
        }
    }
}
