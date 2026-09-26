using StatiCSharp.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace StatiCSharp;

public partial class WebsiteManager : IWebsiteManager
{
    /// <summary>
    /// Writes a file to disc.
    /// </summary>
    /// <param name="path">The traget directory-path.</param>
    /// <param name="filename">The filename for the file.</param>
    /// <param name="content">The content the file will have.</param>
    /// <param name="gitMode">Optional. If true, an existing file is only rewritten if the content changes.</param>
    private void WriteFile(string path, string filename, string content, bool gitMode = false)
    {
        string filePath = Path.Combine(path, filename);
        if (gitMode)
        {
            if (File.Exists(filePath))
            {
                string oldFile = File.ReadAllText(filePath);
                if (oldFile != content)
                {
                    File.WriteAllText(filePath, content);
                }
            }
            else
            {
                File.WriteAllText(filePath, content);
            }
        }
        else
        {
            File.WriteAllText(filePath, content);
        }
    }

    /// <summary>
    /// Writes a file to disc, asynchronously.
    /// </summary>
    /// <param name="path">The traget directory path.</param>
    /// <param name="filename">The filename for the file.</param>
    /// <param name="content">The content of the file.</param>
    /// <param name="gitMode">Optional. If true, an existing file is only rewritten if the content changes.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    private async Task WriteFileAsync(string path, string filename, string content, bool gitMode = false)
    {
        string filePath = Path.Combine(path, filename);

        if (gitMode && File.Exists(filePath))
        {
            string oldFile = await File.ReadAllTextAsync(filePath);
            
            if (oldFile == content)
                return;
        }
        
        await File.WriteAllTextAsync(filePath, content);
    }
}
