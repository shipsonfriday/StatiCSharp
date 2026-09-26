using StatiCSharp.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace StatiCSharp;

public partial class WebsiteManager : IWebsiteManager
{
    /// <summary>
    /// Cleans up the Output directory from HTML files that have no corresponding markdown file.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous clean up operation.</returns>
    private async Task CleanUpAsync()
    {
        await cleanUpDirectory(Output);


        async Task cleanUpDirectory(string directory)
        {
            if (!PathDirectory.Contains(directory))
            {
                // Delete only files named index.html. Other files could be resources!
                if (File.Exists(Path.Combine(directory, "index.html")))
                {
                    File.Delete(Path.Combine(directory, "index.html"));
                }
            }
            if (Directory.GetDirectories(directory).Length == 0 && Directory.GetFiles(directory).Length == 0)
            {
                // Do not delete output directory!
                if (directory != Output)
                {
                    Directory.Delete(directory);
                }
            }
            else
            {
                foreach (string subdir in Directory.GetDirectories(directory))
                {
                    await cleanUpDirectory(subdir);
                }
            }
            
        }
    }
}
