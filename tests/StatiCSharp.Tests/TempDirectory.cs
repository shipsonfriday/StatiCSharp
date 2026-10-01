using System;
using System.IO;

namespace StatiCSharp.Tests;

/// <summary>
/// A throwaway directory, removed again when the test is done.
/// Parts of the library only take paths, so the tests need real files on disk.
/// </summary>
internal sealed class TempDirectory : IDisposable
{
    /// <summary>
    /// The absolute path of the directory.
    /// </summary>
    public string Path { get; }

    public TempDirectory()
    {
        Path = System.IO.Path.Combine(
            System.IO.Path.GetTempPath(),
            $"staticsharp-tests-{Guid.NewGuid():N}");

        Directory.CreateDirectory(Path);
    }

    /// <summary>
    /// Writes a file below this directory, creating intermediate directories as needed.
    /// </summary>
    /// <param name="relativePath">Path relative to this directory, with forward slashes.</param>
    /// <param name="lines">The lines of the file, joined by a line feed.</param>
    /// <returns>The absolute path of the file.</returns>
    public string WriteFile(string relativePath, params string[] lines)
    {
        string full = System.IO.Path.Combine(Path, relativePath.Replace('/', System.IO.Path.DirectorySeparatorChar));
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(full)!);
        File.WriteAllText(full, string.Join("\n", lines));

        return full;
    }

    /// <summary>
    /// Writes a file of raw bytes below this directory, for the cases where a resource is
    /// not text.
    /// </summary>
    /// <param name="relativePath">Path relative to this directory, with forward slashes.</param>
    /// <param name="bytes">The content of the file.</param>
    /// <returns>The absolute path of the file.</returns>
    public string WriteBytes(string relativePath, byte[] bytes)
    {
        string full = System.IO.Path.Combine(Path, relativePath.Replace('/', System.IO.Path.DirectorySeparatorChar));
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(full)!);
        File.WriteAllBytes(full, bytes);

        return full;
    }

    /// <summary>
    /// Creates an empty directory below this one and returns its absolute path.
    /// </summary>
    /// <param name="relativePath">Path relative to this directory, with forward slashes.</param>
    public string EmptyDirectory(string relativePath)
    {
        string full = System.IO.Path.Combine(Path, relativePath.Replace('/', System.IO.Path.DirectorySeparatorChar));
        Directory.CreateDirectory(full);

        return full;
    }

    public void Dispose()
    {
        if (Directory.Exists(Path))
        {
            Directory.Delete(Path, recursive: true);
        }
    }
}
