using System.Collections.Concurrent;

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
/// The bookkeeping records every file the run produces, written or copied. Everything else
/// in the output directory did not come from this website and is removed, which is what
/// makes a deleted markdown file or a renamed resource disappear from the output. The
/// preserved names are the exception, see <see cref="AlwaysPreserved"/>.
/// </para>
/// <para>
/// It is concurrent because the sites are written through
/// <see cref="Task.WhenAll(Task[])"/>. Today every record happens before the first await and
/// therefore in sequence, so nothing depends on it - which is exactly why it is expressed in
/// the type rather than left to the position of an await in another file.
/// </para>
/// </summary>
internal sealed class OutputWriter
{
    /// <summary>
    /// Never deleted, whatever the run produces.
    /// <para>
    /// <c>.git</c> because a published output directory often is the repository it is
    /// deployed from - clearing it would destroy the history. <c>.nojekyll</c> and
    /// <c>CNAME</c> because GitHub Pages reads them and nothing generates them.
    /// </para>
    /// </summary>
    internal static readonly string[] AlwaysPreserved = [".git", ".nojekyll", "CNAME"];

    private readonly ConcurrentDictionary<string, byte> _producedFiles = new(StringComparer.Ordinal);
    private readonly HashSet<string> _preserved;
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
    /// <param name="alsoPreserve">
    /// Names to keep besides <see cref="AlwaysPreserved"/>. Null counts as none.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="output"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="output"/> is empty or only whitespace.</exception>
    internal OutputWriter(string output, bool onlyWriteWhatChanged, IEnumerable<string>? alsoPreserve = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(output);

        _output = Path.GetFullPath(output);
        _onlyWriteWhatChanged = onlyWriteWhatChanged;

        // Case insensitive: deleting someone's CNAME because they wrote it lowercase is the
        // expensive mistake here, keeping a file one did not mean to keep is not.
        _preserved = new HashSet<string>(AlwaysPreserved, StringComparer.OrdinalIgnoreCase);
        if (alsoPreserve is not null)
        {
            _preserved.UnionWith(alsoPreserve);
        }
    }

    /// <summary>
    /// Whether the given file was written or copied during this run, and is therefore part
    /// of the website.
    /// </summary>
    /// <param name="filePath">The path of the file.</param>
    internal bool Produced(string filePath) => _producedFiles.ContainsKey(Path.GetFullPath(filePath));

    /// <summary>
    /// Whether a file or directory of this name is kept even though the run did not produce it.
    /// </summary>
    /// <param name="path">The path of the file or directory.</param>
    internal bool IsPreserved(string path) => _preserved.Contains(Path.GetFileName(path.TrimEnd(
        Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)));

    /// <summary>
    /// Writes a file into the output.
    /// </summary>
    /// <param name="path">The target directory.</param>
    /// <param name="filename">The filename.</param>
    /// <param name="content">The content of the file.</param>
    /// <returns>
    /// False if this run already wrote that file. The file is written either way, so the
    /// later site wins - the caller reports it, because it means two sites claim one url.
    /// </returns>
    internal async Task<bool> WriteAsync(string path, string filename, string content)
    {
        string filePath = Path.Combine(path, filename);
        bool isTheFirst = Record(filePath);

        if (_onlyWriteWhatChanged && File.Exists(filePath))
        {
            string existing = await File.ReadAllTextAsync(filePath);

            if (existing == content)
            {
                return isTheFirst;
            }
        }

        await File.WriteAllTextAsync(filePath, content);
        return isTheFirst;
    }

    /// <summary>
    /// Deletes everything inside the output directory except the preserved names, without
    /// deleting the directory itself.
    /// </summary>
    internal void Clear()
    {
        foreach (string file in Directory.GetFiles(_output).Where(file => !IsPreserved(file)))
        {
            File.Delete(file);
        }

        foreach (string subdirectory in Directory.GetDirectories(_output).Where(directory => !IsPreserved(directory)))
        {
            Directory.Delete(subdirectory, true);
        }
    }

    /// <summary>
    /// Copies a directory with all its files and subdirectories into the output directory.
    /// The copied files count as produced by this run, so the clean up keeps them. A file
    /// whose content is already in place is not copied again.
    /// <para>
    /// <paramref name="sourceDir"/> must not hold the output directory, or the output is
    /// copied into itself - one level deeper on every run, since the copies count as produced
    /// and survive the clean up. <see cref="EnvironmentCheck"/> refuses such a run before
    /// anything is written.
    /// </para>
    /// </summary>
    /// <param name="sourceDir">The directory to copy from.</param>
    /// <returns>A task that represents the asynchronous copying operation.</returns>
    /// <exception cref="DirectoryNotFoundException"><paramref name="sourceDir"/> does not exist.</exception>
    internal Task CopyIntoOutputAsync(string sourceDir) => CopyAllAsync(sourceDir, _output);

    /// <summary>
    /// Removes everything in the output directory this run did not produce, and any directory
    /// left empty by that. The preserved names stay, and a preserved directory is not even
    /// looked into.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous clean up operation.</returns>
    internal Task CleanUpAsync() => Task.Run(() => CleanUp(_output));

    private void CleanUp(string directory)
    {
        foreach (string file in Directory.GetFiles(directory))
        {
            if (!Produced(file) && !IsPreserved(file))
            {
                File.Delete(file);
            }
        }

        foreach (string subdirectory in Directory.GetDirectories(directory))
        {
            if (IsPreserved(subdirectory))
            {
                continue;
            }

            CleanUp(subdirectory);

            if (Directory.GetFileSystemEntries(subdirectory).Length == 0)
            {
                Directory.Delete(subdirectory);
            }
        }
    }

    private bool Record(string filePath) => _producedFiles.TryAdd(Path.GetFullPath(filePath), 0);

    /// <summary>
    /// Whether the file at <paramref name="destination"/> already holds exactly what
    /// <paramref name="source"/> holds.
    /// <para>
    /// Compared byte by byte rather than as text, because resources are images, fonts and
    /// archives as often as they are style sheets. The length settles almost every case
    /// before a single byte is read.
    /// </para>
    /// </summary>
    private static async Task<bool> HasTheSameContentAsync(FileInfo source, string destination)
    {
        FileInfo target = new(destination);

        if (!target.Exists || target.Length != source.Length)
        {
            return false;
        }

        const int bufferSize = 64 * 1024;
        byte[] fromSource = new byte[bufferSize];
        byte[] fromTarget = new byte[bufferSize];

        await using FileStream sourceStream = source.OpenRead();
        await using FileStream targetStream = target.OpenRead();

        while (true)
        {
            int read = await sourceStream.ReadAtLeastAsync(fromSource, bufferSize, throwOnEndOfStream: false);
            int alsoRead = await targetStream.ReadAtLeastAsync(fromTarget, bufferSize, throwOnEndOfStream: false);

            if (read != alsoRead)
            {
                // Cannot happen for two files of the same length, but a difference in what
                // was read is a difference either way.
                return false;
            }

            if (read == 0)
            {
                return true;
            }

            if (!fromSource.AsSpan(0, read).SequenceEqual(fromTarget.AsSpan(0, read)))
            {
                return false;
            }
        }
    }

    private async Task CopyAllAsync(string sourceDir, string destinationDir)
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
            string destination = Path.Combine(destinationDir, file.Name);
            Record(destination);

            if (_onlyWriteWhatChanged && await HasTheSameContentAsync(file, destination))
            {
                continue;
            }

            file.CopyTo(destination, true);
        }

        foreach (DirectoryInfo subDir in dirs)
        {
            await CopyAllAsync(subDir.FullName, Path.Combine(destinationDir, subDir.Name));
        }
    }
}
