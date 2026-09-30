using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace StatiCSharp;

/// <summary>
/// Owns everything that touches the output directory during one generating run: writing the
/// sites, copying resources into it, clearing it, and removing what is left over.
/// <para>
/// Created once per run, which is what makes the path bookkeeping correct. As a field on the
/// manager it survived the run, so a second <c>MakeAsync</c> on the same instance considered
/// every path from the first run already taken and warned about all of them.
/// </para>
/// <para>
/// The bookkeeping is concurrent because the sites are written through
/// <see cref="Task.WhenAll(Task[])"/>. Today every claim happens before the first await and
/// therefore in sequence, so nothing depends on it - which is exactly why it is expressed in
/// the type rather than left to the position of an await in another file.
/// </para>
/// </summary>
internal sealed class OutputWriter
{
    private readonly ConcurrentDictionary<string, byte> _claimedPaths = new();
    private readonly string _output;
    private readonly bool _onlyWriteWhatChanged;

    /// <summary>
    /// Starts a writer for one run.
    /// </summary>
    /// <param name="output">The absolute path of the output directory.</param>
    /// <param name="onlyWriteWhatChanged">
    /// If true, an existing file is left alone when its content did not change, and the
    /// output directory is not cleared before writing.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="output"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="output"/> is empty or only whitespace.</exception>
    internal OutputWriter(string output, bool onlyWriteWhatChanged)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(output);

        _output = output;
        _onlyWriteWhatChanged = onlyWriteWhatChanged;
    }

    /// <summary>
    /// Records that a directory is being written to.
    /// </summary>
    /// <param name="path">The directory.</param>
    /// <returns>False if another site already claimed it.</returns>
    internal bool ClaimPath(string path) => _claimedPaths.TryAdd(path, 0);

    /// <summary>
    /// Whether anything was written to the given directory during this run.
    /// </summary>
    /// <param name="path">The directory.</param>
    internal bool WasWrittenTo(string path) => _claimedPaths.ContainsKey(path);

    /// <summary>
    /// Writes a file into the output.
    /// </summary>
    /// <param name="path">The target directory.</param>
    /// <param name="filename">The filename.</param>
    /// <param name="content">The content of the file.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    internal async Task WriteAsync(string path, string filename, string content)
    {
        string filePath = Path.Combine(path, filename);

        if (_onlyWriteWhatChanged && File.Exists(filePath))
        {
            string existing = await File.ReadAllTextAsync(filePath);

            if (existing == content)
            {
                return;
            }
        }

        await File.WriteAllTextAsync(filePath, content);
    }

    /// <summary>
    /// Deletes everything inside the output directory, without deleting the directory itself.
    /// </summary>
    internal void Clear() => DeleteContentsOf(_output);

    /// <summary>
    /// Copies a directory with all its files and subdirectories into the output directory.
    /// </summary>
    /// <param name="sourceDir">The directory to copy from.</param>
    /// <returns>A task that represents the asynchronous copying operation.</returns>
    /// <exception cref="DirectoryNotFoundException"><paramref name="sourceDir"/> does not exist.</exception>
    internal Task CopyIntoOutputAsync(string sourceDir) => CopyAllAsync(sourceDir, _output);

    /// <summary>
    /// Removes the html files in the output that have no corresponding markdown file, and any
    /// directory left empty by that.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous clean up operation.</returns>
    internal async Task CleanUpAsync() => await CleanUpDirectoryAsync(_output);

    private async Task CleanUpDirectoryAsync(string directory)
    {
        if (!WasWrittenTo(directory))
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
            if (directory != _output)
            {
                Directory.Delete(directory);
            }
        }
        else
        {
            foreach (string subdir in Directory.GetDirectories(directory))
            {
                await CleanUpDirectoryAsync(subdir);
            }
        }
    }

    private static void DeleteContentsOf(string path)
    {
        DirectoryInfo directory = new(path);

        foreach (FileInfo file in directory.GetFiles())
        {
            file.Delete();
        }

        foreach (DirectoryInfo subdirectory in directory.GetDirectories())
        {
            subdirectory.Delete(true);
        }
    }

    private static async Task CopyAllAsync(string sourceDir, string destinationDir)
    {
        // https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
        var dir = new DirectoryInfo(sourceDir);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
        }

        // Cache directories before we start copying.
        DirectoryInfo[] dirs = dir.GetDirectories();

        Directory.CreateDirectory(destinationDir);

        foreach (FileInfo file in dir.GetFiles())
        {
            file.CopyTo(Path.Combine(destinationDir, file.Name), true);
        }

        foreach (DirectoryInfo subDir in dirs)
        {
            await CopyAllAsync(subDir.FullName, Path.Combine(destinationDir, subDir.Name));
        }
    }
}
