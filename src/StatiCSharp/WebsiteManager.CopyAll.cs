using StatiCSharp.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace StatiCSharp;

public partial class WebsiteManager : IWebsiteManager
{
    /// <summary>
    /// Asynchronously copies all directories and files (incl. subfolders and -files) from the source directory to the destination directory.
    /// </summary>
    /// <param name="sourceDir">Source directory</param>
    /// <param name="destinationDir">Source directory</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous copying operation.</returns>
    /// <exception cref="DirectoryNotFoundException"></exception>
    private async Task CopyAllAsync(string sourceDir, string destinationDir)
    {
        // https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories


        // Get information about the source directory
        var dir = new DirectoryInfo(sourceDir);

        // Check if the source directory exists
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath, true);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (true)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                await CopyAllAsync(subDir.FullName, newDestinationDir);
            }
        }
    }
}
